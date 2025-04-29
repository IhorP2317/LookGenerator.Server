using System.Globalization;
using System.Net.Http.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using LookGenerator.Infrastructure.Models.Responses.Looks;
using LookGenerator.Infrastructure.Models.Responses.Products;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LookGenerator.Infrastructure.Jobs.Definitions;

public class BershkaLookParsingJob(IHttpClientFactory httpClientFactory, IApplicationDbContext applicationDbContext)
    : IJob
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("NoBaseUriClient");


    private static string LooksUrl =>
        "https://distillery.pixlee.co/api/v2/albums/60934352/photos?per_page=200&filters={\"has_permission\":true,\"has_product\":true,\"in_categories\":[66486573,66486574],\"not_in_categories\":[71095956]}&api_key=4zeyDeJEOsPuMRBBgB6D&appId=1&storeId=45109563&languageId=-1&locale=uk_UA";

    private static string ProductArrayUrl =>
        "https://www.bershka.com/itxrest/3/catalog/store/45109563/40259532/productsArray?appId=1&languageId=-1&locale=uk_UA";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var page = context.MergedJobDataMap.GetIntValue("Page");
            if (page == 0) return;

            var preparedLookUrl = $"{LooksUrl}&page={page}";
            var lookResponse = await _httpClient.GetAsync(preparedLookUrl);
            lookResponse.EnsureSuccessStatusCode();
            var lookData = await lookResponse.Content.ReadFromJsonAsync<LookPhotos>();
            if (lookData?.Data == null) return;

            // Prepare SKU to metadata
            var skuToAttributeCategories = new Dictionary<string, List<string>>();
            foreach (var look in lookData.Data)
            {
                var attributeOptions = look.Categories
                    .Where(c => c.StartsWith("Chicos-") || c.StartsWith("Chicas-"))
                    .Select(c =>
                    {
                        var name = c.Split('-')[1].ToLowerInvariant();
                        return name switch
                        {
                            "streetstyle" => "StreetWear",
                            "viral" => "Trendy",
                            _ => name
                        };
                    }).ToList();

                foreach (var product in look.Products.Where(product => !string.IsNullOrWhiteSpace(product.Sku)))
                {
                    if (!skuToAttributeCategories.TryGetValue(product.Sku, out var existing))
                    {
                        skuToAttributeCategories[product.Sku] = attributeOptions;
                    }
                    else
                    {
                        existing.AddRange(attributeOptions);
                    }
                }
            }

            var masterSizingIdentifiers = await applicationDbContext.MasterSizeIdentifiers.ToListAsync();
            var attributeOptionsLookup = await applicationDbContext.AttributeOptions
                .ToDictionaryAsync(a => a.Name.ToLower(), a => a.Id);

            foreach (var skuChunk in skuToAttributeCategories.Keys.Chunk(200))
            {
                var joinedSkus = string.Join(",", skuChunk);
                var skuUrl = $"{ProductArrayUrl}&referenceIds={joinedSkus}";

                var response = await _httpClient.GetAsync(skuUrl);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadFromJsonAsync<ProductArrayResponse>();
                var productArray = data?.Products.Where(p => p.Description == null && !string.IsNullOrEmpty(p.NameEn))
                    .ToList();
                if (productArray is null || productArray.Count == 0) continue;
                var colorIds = productArray
                    .SelectMany(p =>
                        p.Detail?.Colors ?? p.BundleProductSummaries?.FirstOrDefault()?.Detail?.Colors ?? [])
                    .Select(c => int.Parse(c.Id))
                    .Distinct()
                    .ToList();

                var colourLookup = await applicationDbContext.Colours
                    .Where(c => colorIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id);

                for (var i = 0; i < productArray.Count; i++)
                {
                    var sku = skuChunk[i];
                    if (!skuToAttributeCategories.TryGetValue(sku, out var pixleeData)) continue;
                    var attributeIds = pixleeData
                        .Distinct()
                        .Where(attributeOptionsLookup.ContainsKey)
                        .Select(name => attributeOptionsLookup[name])
                        .ToList();

                    await ProcessProductElementAsync(
                        productArray[i],
                        attributeIds,
                        masterSizingIdentifiers,
                        colourLookup
                    );
                }

                await applicationDbContext.SaveChangesAsync();
            }
            if (lookData.Next)
            {
                var scheduler = context.Scheduler;
                var nextPage = page + 1;
                var jobKey = new JobKey($"BershkaLookParsingJob_Page_{nextPage}");

                var jobDetail = JobBuilder.Create<BershkaLookParsingJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("Page", nextPage)
                    .StoreDurably()
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"BershkaLookParsingTrigger_Page_{nextPage}")
                    .StartNow()
                    .ForJob(jobDetail)
                    .Build();

                await scheduler.AddJob(jobDetail, true);
                await scheduler.ScheduleJob(trigger);
            }
            else
            {
                Console.WriteLine("Jobs is done!");
                var scheduler = context.Scheduler;
                var jobKey = new JobKey(nameof(BershkaColorNormalizationJob));
                var jobDetail = JobBuilder.Create<BershkaColorNormalizationJob>()
                    .WithIdentity(jobKey)
                    .StoreDurably()
                    .Build();
                
                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{nameof(BershkaColorNormalizationJob)}Trigger")
                    .StartNow()
                    .ForJob(jobDetail)
                    .Build();
                
                await scheduler.AddJob(jobDetail, true);
                await scheduler.ScheduleJob(trigger);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    private async Task ProcessProductElementAsync(ProductElement productElement,
        List<Guid> attributeOptionIds,
        List<MasterSizeIdentifier> masterSizingIdentifiers,
        Dictionary<int, Colour> colourLookup)
    {
        var sectionCategory = await EnsureCategoryAsync(productElement.SectionNameEn, null);
        if (sectionCategory == null) return;
        var typeCategory = await EnsureCategoryAsync(productElement.ProductType, sectionCategory.Id);
        var familyCategory = !string.Equals(productElement.FamilyNameEn, productElement.ProductType,
            StringComparison.OrdinalIgnoreCase)
            ? await EnsureCategoryAsync(productElement.FamilyNameEn, typeCategory?.Id)
            : typeCategory;

        var subFamilyCategory = !string.Equals(productElement.SubFamilyNameEn, productElement.FamilyNameEn,
            StringComparison.OrdinalIgnoreCase)
            ? await EnsureCategoryAsync(productElement.SubFamilyNameEn, familyCategory?.Id)
            : familyCategory;

        var finalCategory = subFamilyCategory ?? familyCategory ?? typeCategory ?? sectionCategory;
        var product = await applicationDbContext.Products.Include(p => p.Items).ThenInclude(i => i.Links)
            .Include(p => p.Items).ThenInclude(i => i.Images)
            .Include(p => p.Items).ThenInclude(i => i.Variations).ThenInclude(v => v.MasterSizeIdentifier)
            .FirstOrDefaultAsync(p => p.ExternalId == productElement.Id);

        if (product == null)
        {
            var correctedIsTop = productElement.IsTop;

            if (productElement.SectionNameEn != "MEN")
            {
                if (productElement.SubFamilyNameEn == "Swimwear")
                {
                    correctedIsTop =
                        productElement.NameEn.Contains("bottoms", StringComparison.CurrentCultureIgnoreCase) ? 0 : 1;
                }
                if (productElement.SubFamilyNameEn == "Underwear")
                {
                    if (productElement.NameEn.Contains("top", StringComparison.CurrentCultureIgnoreCase) ||
                        productElement.NameEn.Contains("dress", StringComparison.CurrentCultureIgnoreCase) ||
                        productElement.NameEn.Contains("bra", StringComparison.CurrentCultureIgnoreCase))
                    {
                        correctedIsTop = 1;
                    }
                    else
                    {
                        correctedIsTop = 0;
                    }
                   
                }
            }
            else
            {
                if (productElement.SubFamilyNameEn == "Underwear")
                {
                    correctedIsTop = 0;
                }
            }

            if (productElement.ProductType == "Footwear" || productElement.SubFamilyNameEn == "Socks")
            {
                correctedIsTop = 3;
            }

            product = new Product
            {
                ExternalId = productElement.Id,
                Name = productElement.NameEn,
                Description = productElement.Detail?.Description,
                CategoryId = finalCategory.Id,
                BodyZone = (ProductBodyZone)correctedIsTop
            };
            await applicationDbContext.Products.AddAsync(product);

            product.ProductAttributeOptions.AddRange(attributeOptionIds.Select(attrId => new ProductAttributeOption
            {
                Product = product,
                AttributeOptionId = attrId
            }));
        }

        var colors = productElement.Detail?.Colors ??
                     productElement.BundleProductSummaries?.FirstOrDefault()?.Detail?.Colors ?? [];
        var mediaGroup = productElement.Detail?.Xmedia ??
                         productElement.BundleProductSummaries?.FirstOrDefault()?.Detail?.Xmedia;
        foreach (var color in colors)
        {
            if (!colourLookup.TryGetValue(int.Parse(color.Id), out var colourEntity))
            {
                colourEntity = new Colour { Id = int.Parse(color.Id), Name = color.Name };
                colourLookup[colourEntity.Id] = colourEntity;
                if (applicationDbContext.Colours.Local.All(c => c.Id != colourEntity.Id))
                {
                    await applicationDbContext.Colours.AddAsync(colourEntity);
                }
            }

            var existingItem = product.Items.FirstOrDefault(i => i.ColourId == colourEntity.Id);
            if (existingItem == null)
            {
                existingItem = new ProductItem { ColourId = colourEntity.Id, Colour = colourEntity };
                var itemLink = new ProductLink
                {
                    Url = $"https://www.bershka.com/ua/{productElement.ProductUrl}.html?colorId={color.Id}",
                    RegionName = "ua_UA"
                };
                existingItem.Links.Add(itemLink);
                product.Items.Add(existingItem);
            }

            var imageUrls = mediaGroup?
                .FirstOrDefault(m => m.ColorCode == color.Id)?
                .XmediaItems
                .SelectMany(x => x.Medias)
                .Where(m => m.IdMedia.EndsWith("_2_4_"))
                .Select(m => m.Url ?? (m.ExtraInfo?.Url != null ? $"https://static.bershka.net/4/photos2{m.ExtraInfo.Url}" : null))
                .Where(url => !string.IsNullOrEmpty(url))
                .Select(url => new Uri(url!, UriKind.RelativeOrAbsolute).GetLeftPart(UriPartial.Path))
                .Distinct()
                .ToList();




            if (imageUrls is { Count: > 0 })
            {
                foreach (var imageUrl in imageUrls.Where(imageUrl =>
                             existingItem.Images.All(existingImage => existingImage.ImageUrl != imageUrl)))
                {
                    existingItem.Images.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ProductItem = existingItem,
                        ProductOnly = true
                    });
                }
            }

            if (color.Sizes is not { Count: > 0 }) continue;
            {
                foreach (var size in color.Sizes)
                {
                    var masterSizeIdentifier = masterSizingIdentifiers
                        .FirstOrDefault(m => m.Identifier == size.MastersSizeId);

                    if (masterSizeIdentifier == null)
                    {
                        masterSizeIdentifier = new MasterSizeIdentifier
                        {
                            Identifier = size.MastersSizeId
                        };
                        applicationDbContext.MasterSizeIdentifiers.Add(masterSizeIdentifier);
                        masterSizingIdentifiers.Add(masterSizeIdentifier);
                    }

                    var preparedPrice = size.Price.Insert(size.Price.Length - 2, ".");
                    var parsedPrice = decimal.Parse(preparedPrice,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                        CultureInfo.InvariantCulture);

                    var existingVariation = existingItem.Variations
                        .FirstOrDefault(v => v.MasterSizeIdentifier.Identifier == size.MastersSizeId);

                    if (existingVariation != null)
                    {
                        if (existingVariation.Price == parsedPrice) continue;
                        existingVariation.Price = parsedPrice;
                        applicationDbContext.ProductVariations.Update(existingVariation);
                    }
                    else
                    {
                        existingItem.Variations.Add(new ProductVariation
                        {
                            Price = parsedPrice,
                            MasterSizeIdentifier = masterSizeIdentifier,
                            ProductItem = existingItem,
                            Size = size.Name,
                            IsInStock = size.IsBuyable,
                            SizeType = size.SizeType
                        });
                    }
                }
            }
        }
    }

    private async Task<ProductCategory?> EnsureCategoryAsync(string name, Guid? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        if (parentId.HasValue)
        {
            var parentExists = await applicationDbContext.ProductCategories
                .AnyAsync(c => c.Id == parentId.Value);

            if (!parentExists)
            {
                throw new InvalidOperationException($"Parent category with ID '{parentId.Value}' does not exist.");
            }
        }

        var loweredName = name.ToLower();
        var category = await applicationDbContext.ProductCategories
            .FirstOrDefaultAsync(c => c.Name.ToLower() == loweredName
                                      && c.ParentCategoryId == parentId);

        if (category != null) return category;

        category = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = name,
            ParentCategoryId = parentId
        };

        await applicationDbContext.ProductCategories.AddAsync(category);
        await applicationDbContext.SaveChangesAsync();

        return category;
    }
}