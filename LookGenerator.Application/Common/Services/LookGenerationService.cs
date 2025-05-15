using System.Text.Json;
using System.Text.Json.Serialization;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Constants;
using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Common.Mappers;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Common.Services;

public class LookGenerationService(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IClient client) : ILookGenerationService
{
    public async Task<LookGenerationRequest>
        GetDenormalizedProductsAsync(string gender,
            ICollection<Guid> attributeOptionIds,
            List<Guid> topSizeIds,
            List<Guid> bottomSizeIds,
            List<Guid> footwearSizeIds,
            CancellationToken cancellationToken
        )
    {
        var rootCategoryIds = await applicationDbContext.ProductCategories
            .Where(pc => gender.ToLower() == pc.Name.ToLower() && pc.ParentCategoryId == null)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var allCategoryIds = new HashSet<Guid>(rootCategoryIds);
        foreach (var rootId in rootCategoryIds)
        {
            await AddDescendantCategoryIds(rootId, allCategoryIds, cancellationToken);
        }

        var products = await LoadAllProductsForCategoriesAsync(
            allCategoryIds,
            attributeOptionIds,
            topSizeIds,
            bottomSizeIds,
            footwearSizeIds,
            cancellationToken);

        return new LookGenerationRequest(Products: products);
    }

    public async Task<LooksGenerationResponse> GenerateLooksAsync(
        LookGenerationRequest categoryHierarchy,
        string prioritizedColorList,
        string? fixedProductJson,
        CancellationToken ct)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var hierarchyJson = JsonSerializer.Serialize(categoryHierarchy, jsonOptions);

        var instructions = GenerateInstructions(prioritizedColorList, fixedProductJson);
        var initialMessage = $@"Generate stylish fashion looks based on the uploaded product catalog data.
Prioritize items with these colour names: {string.Join(", ", prioritizedColorList)} when possible.
Try to generate distinct looks across different colour palette types (e.g., Monochromatic, Triadic, etc.).

## Reminder:
Every look must contain product item:
All valid productItem IDs to use are embedded in the JSON below as ""id"" fields under ""productItems"".
DO NOT make up any IDs. Use them exactly as given.";

        var combinedPrompt = $"""
                              {instructions}
                              Input Data:
                              {hierarchyJson}
                              {initialMessage}
                              """;

        var responseJson = await client.SendAsync(combinedPrompt, LookGenerationConstants.OutputResultJson, ct);

        return JsonSerializer.Deserialize<LooksGenerationResponse>(responseJson, jsonOptions)!;
    }

    private string GenerateInstructions(string prioritizedColorList, string? fixedProductJson)
    {
        var fixedItemRule = fixedProductJson != null
            ? $"8. You must include this  productItem in every generated look: \"{fixedProductJson}\"."
            : "";

        return $$"""
                 You are a professional fashion stylist assistant. Your task is to generate fashion looks based strictly on the provided JSON product catalog.

                 # RULES

                 ## Catalog structure:
                 - products[]: A flat collection of all available fashion products.
                   - Each product includes:
                     - id: Unique identifier for the product
                     - name: Product name
                     - bodyZone: one of "UpperBody", "LowerBody", "Feet", or "HeadOrExtras"
                     - categories: List of category names this product belongs to (e.g., "Clothing", "Footwear", "Tops", "Dresses")
                     - productItems[]: Specific variations of the product, each with:
                       - id: Unique GUID identifier for this specific product item
                       - colour: Object containing id and name of the color


                 {{LookGenerationConstants.OutputFormat}}
                 ## REQUIRED for every look:
                 1. Must contain at least one clothing item, and either:
                    - A single full-body item (e.g. a dress or jumpsuit), OR
                    - A valid combination of one UpperBody and one LowerBody item.
                 2. Must contain exactly one `Feet` item (Footwear is required).
                 3. Minimum of 3 items or more.
                 4. Only one item per bodyZone, except:
                    - Up to 2 items from UpperBody if layered correctly.
                    - If one of them is a Jacket or Coat, it MUST be layered over another UpperBody item.
                    - Never use only a jacket or only a coat as the sole UpperBody item.
                 5. Accessories are optional, but encouraged. If included:
                    - Do not repeat the same accessory type (e.g., no two scarves).
                    - Do not exceed 3 accessory items in a single look.
                 6. No two items from the same product.id.
                 7. Each look must **include all the colours specified**: {{prioritizedColorList}}.
                 - If "multicolor" is among the specified colours but no matching product items exist, select three to four random colours from available product item colours instead.
                 - If it is not possible to satisfy this colour requirement for a look, skip generating that look.
                 {{fixedItemRule}}
                 ## Colour Harmony Rules (based on Colour Wheel):
                 Use these predefined color families to support harmony in looks:

                    - Neutrals: ["white", "black", "grey", "beige", "transparent"]
                    - Warm: ["red", "orange", "yellow", "brown", "pink"]
                    - Cool: ["blue", "green", "violet"]

                 Every look must follow one of the following palette types:

                 - Monochromatic: Various tints and shades of a single hue (e.g. Light Red, Medium Red, Dark Red)
                 - Analogous: 2–3 colors that are adjacent on the color wheel (e.g. Yellow, Yellow-Orange, Orange)
                 - Triadic: 3 evenly spaced hues on the wheel forming a triangle (e.g. Red, Blue, Yellow)
                 - Split-Complementary: 1 base color + 2 adjacent to its complement (e.g. Blue + Red-Orange and Yellow-Orange)
                 - Tetradic: Two complementary color pairs (e.g. Red + Green and Blue + Orange)

                 Rules:
                 - Max 3 hues per look
                 - Repeat hues across items for visual harmony
                 - Do not mix clashing or unrelated colors

                 ## ID Rules (ABSOLUTELY STRICT):

                 - You must only use existing `productItem.id` values from the input JSON.
                 - Each productItem.id is a GUID string (e.g. `"5a98131b-1906-4571-a644-e6a6558a1c47"`).
                 - Do not invent or fabricate IDs.
                 - Do not use product IDs (like product.id`) or colour information (like `colour.name or `colour.id`).
                 - All valid IDs are already listed in productItems[] in each product.

                 ## Additional Requirement:
                 - If no valid looks can be generated (`"looks"` array is empty), include a description, that describes he cause of failure.
                 - For valid look entries, ensure the description is null.

                 ## Final Output:
                 - Strictly return valid JSON with the "looks" array.
                 - NO commentary, notes, placeholders, or partial/incomplete looks.
                 - Return only complete and valid looks that follow ALL rules.
                 """;
    }

    public async Task<List<LookResponse>> MapAndSaveLooksAsync(
        LooksGenerationResponse? response,
        LookGenerationRequest categoryHierarchy,
        Dictionary<Guid, Dictionary<string, SizeOption>> topSizeMap,
        Dictionary<Guid, Dictionary<string, SizeOption>> bottomSizeMap,
        Dictionary<Guid, Dictionary<string, SizeOption>> footwearSizeMap,
        CancellationToken cancellationToken)
    {
        var looksResponse = new List<LookResponse>();
        foreach (var (colorPalette, name, productItemIds, description) in response?.Looks ??
                                                                          Enumerable.Empty<LookGenerationResponse>())
        {
            var skipLook = false;
            var productDescriptions =
                new Dictionary<Guid, (Dictionary<string, List<string>> attributes, List<string> categories)>();
            foreach (var productId in productItemIds)
            {
                var product =
                    categoryHierarchy.Products.FirstOrDefault(p => p.ProductItems.Any(pi => pi.Id == productId));
                if (product == null)
                {
                    skipLook = true;
                    break;
                }

                if (productDescriptions.ContainsKey(product.Id))
                    continue;
                var categories = product.Categories
                    .Distinct()
                    .ToList();
                var attributes = await applicationDbContext.AttributeOptions
                    .Include(ao => ao.AttributeType)
                    .Include(ao => ao.ProductAttributeOptions)
                    .Where(ao => ao.ProductAttributeOptions.Any(pao => pao.ProductId == product.Id))
                    .GroupBy(ao => ao.AttributeType.Name)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.Select(ao => ao.Name).Distinct()
                            .ToList(), cancellationToken: cancellationToken);
                productDescriptions[product.Id] =
                    new ValueTuple<Dictionary<string, List<string>>, List<string>>(attributes, categories);
            }

            if (skipLook)
                continue;

            var matchingVariations = await applicationDbContext.ProductVariations
                .Include(v => v.ProductItem)
                .ThenInclude(pi => pi.Colour)
                .Include(v => v.ProductItem.Images)
                .Include(v => v.ProductItem.Links)
                .Include(v => v.ProductItem.Product)
                .Where(v => productItemIds.Contains(v.ProductItemId) &&
                            (
                                (v.ProductItem.Product.BodyZone == ProductBodyZone.UpperBody &&
                                 topSizeMap.Keys.Contains(v.MasterSizeIdentifierId)) ||
                                (v.ProductItem.Product.BodyZone == ProductBodyZone.LowerBody &&
                                 bottomSizeMap.Keys.Contains(v.MasterSizeIdentifierId)) ||
                                (v.ProductItem.Product.BodyZone == ProductBodyZone.Feet &&
                                 footwearSizeMap.Keys.Contains(v.MasterSizeIdentifierId)) ||
                                v.ProductItem.Product.BodyZone == ProductBodyZone.HeadOrExtras
                            ))
                .ToListAsync(cancellationToken);


            var lookEntity = new Look
            {
                Name = name,
                Description = description,
                ColorPalette = colorPalette,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserService.UserId == null ? null : Guid.Parse(currentUserService.UserId),
                LookStatus = LookStatus.Draft,
                LookProductVariations = matchingVariations
                    .Select(v => new LookProductVariation { ProductVariationId = v.Id })
                    .ToList()
            };
            await applicationDbContext.Looks.AddAsync(lookEntity, cancellationToken);
            await applicationDbContext.SaveChangesAsync(cancellationToken);
            looksResponse.Add(lookEntity.ToResponse(
                matchingVariations,
                productDescriptions,
                topSizeMap,
                bottomSizeMap,
                footwearSizeMap
            ));
        }

        return looksResponse;
    }

    private async Task<ICollection<ProductRequest>> LoadAllProductsForCategoriesAsync(
        HashSet<Guid> categoryIds,
        ICollection<Guid> attributeOptionIds,
        List<Guid> topMasterSizeIds,
        List<Guid> bottomMasterSizeIds,
        List<Guid> footwearMasterSizeIds,
        CancellationToken cancellationToken)
    {
        var products = await applicationDbContext.Products
            .Include(p => p.ProductAttributeOptions)
            .Where(p => categoryIds.Contains(p.CategoryId) &&
                        attributeOptionIds.All(attrOptId =>
                            p.ProductAttributeOptions.Any(pao => pao.AttributeOptionId == attrOptId)))
            .ToListAsync(cancellationToken);

        var productDictionary = new Dictionary<Guid, ProductRequest>();


        var categoryPathsById = await GetCategoryPathsAsync(cancellationToken);

        foreach (var product in products)
        {
            var productItems = await applicationDbContext.ProductItems
                .Include(pi => pi.Colour)
                .Include(pi => pi.Variations)
                .Include(pi => pi.Product)
                .Where(pi => pi.ProductId == product.Id &&
                             (
                                 (pi.Product.BodyZone == ProductBodyZone.UpperBody &&
                                  pi.Variations.Any(v => topMasterSizeIds.Contains(v.MasterSizeIdentifierId)))
                                 ||
                                 (pi.Product.BodyZone == ProductBodyZone.LowerBody &&
                                  pi.Variations.Any(v => bottomMasterSizeIds.Contains(v.MasterSizeIdentifierId)))
                                 ||
                                 (pi.Product.BodyZone == ProductBodyZone.Feet &&
                                  pi.Variations.Any(v => footwearMasterSizeIds.Contains(v.MasterSizeIdentifierId)))
                                 ||
                                 pi.Product.BodyZone == ProductBodyZone.HeadOrExtras
                             ))
                .ToListAsync(cancellationToken);


            if (productItems.Count <= 0) continue;


            var categoryNames = new List<string>();
            if (categoryPathsById.TryGetValue(product.CategoryId, out var categoryPath))
            {
                categoryNames.AddRange(categoryPath);
            }

            foreach (var pi in productItems)
            {
                pi.Colour.Name = pi.Colour.Name.ToLowerInvariant();
            }

            var productRequest = product.ToRequest(
                productItems: productItems.Select(pi => pi.ToRequest()).ToList(),
                categories: categoryNames
            );

            productDictionary[product.Id] = productRequest;
        }

        return productDictionary.Values.ToList();
    }

    private async Task AddDescendantCategoryIds(
        Guid categoryId,
        HashSet<Guid> allCategoryIds,
        CancellationToken cancellationToken)
    {
        var childCategoryIds = await applicationDbContext.ProductCategories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == categoryId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var childId in childCategoryIds.Where(allCategoryIds.Add))
        {
            await AddDescendantCategoryIds(childId, allCategoryIds, cancellationToken);
        }
    }

    private async Task<Dictionary<Guid, List<string>>> GetCategoryPathsAsync(CancellationToken cancellationToken)
    {
        var allCategories = await applicationDbContext.ProductCategories
            .AsNoTracking()
            .Select(c => new ValueTuple<Guid, string, Guid?>(c.Id, c.Name, c.ParentCategoryId))
            .ToListAsync(cancellationToken);
        

        return CategoryHelper.BuildCategoryPaths(allCategories);
    }
}