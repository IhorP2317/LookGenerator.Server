using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.Products;

public record ProductArrayResponse(
    [property: JsonPropertyName("products")]
    List<ProductElement> Products
);

public record ProductElement(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("nameEn")] string NameEn,
    [property: JsonPropertyName("unitsLot")]
    int UnitsLot,
    [property: JsonPropertyName("isTop")] int IsTop,
    [property: JsonPropertyName("sizeSystem")]
    string? SizeSystem,
    [property: JsonPropertyName("sectionNameEN")]
    string SectionNameEn,
    [property: JsonPropertyName("productType")]
    string ProductType,
    [property: JsonPropertyName("productUrl")]
    string ProductUrl,
    [property: JsonPropertyName("familyNameEN")]
    string FamilyNameEn,
    [property: JsonPropertyName("subFamilyNameEN")]
    string SubFamilyNameEn,
    [property: JsonPropertyName("relatedCategories")]
    List<Category>? RelatedCategories,
    [property: JsonPropertyName("detail")] ProductDetail? Detail,
    [property: JsonPropertyName("colors")] List<Color>? Colors,
    [property: JsonPropertyName("tags")] List<string>? Tags,
    [property: JsonPropertyName("bundleProductSummaries")]
    List<ProductElement>? BundleProductSummaries,
    [property: JsonPropertyName("bundleColors")]
    List<Color>? BundleColors
);

public record Category(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("identifier")]
    string Identifier,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("urlCategory")]
    bool UrlCategory
);

public record ProductDetail(
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("colors")] List<Color>? Colors,
    [property: JsonPropertyName("xmedia")] List<XmediaGroup>? Xmedia
);

public record Color(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("sizes")] List<ColorSize>? Sizes
);

public record ColorSize(
    [property: JsonPropertyName("sku")] long Sku,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("isBuyable")]
    bool IsBuyable,
    [property: JsonPropertyName("backSoon")]
    string BackSoon,
    [property: JsonPropertyName("mastersSizeId")]
    string MastersSizeId,
    [property: JsonPropertyName("price")] string Price,
    [property: JsonPropertyName("sizeType")]
    string SizeType,
    [property: JsonPropertyName("country")]
    string Country,
    [property: JsonPropertyName("skuDimensions")]
    List<SkuDimension>? SkuDimensions,
    [property: JsonPropertyName("visibilityValue")]
    string VisibilityValue,
    [property: JsonPropertyName("weight")] string Weight,
    [property: JsonPropertyName("sizeSystem")]
    string SizeSystem
);

public record XmediaGroup(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("xmediaItems")]
    List<XmediaItem> XmediaItems,
    [property: JsonPropertyName("colorCode")]
    string ColorCode
);

public record XmediaItem(
    [property: JsonPropertyName("medias")] List<Media> Medias,
    [property: JsonPropertyName("set")] int Set
);

public record Media(
    [property: JsonPropertyName("format")] int Format,
    [property: JsonPropertyName("clazz")] int Clazz,
    [property: JsonPropertyName("idMedia")]
    string IdMedia,
    [property: JsonPropertyName("timestamp")]
    long Timestamp,
    [property: JsonPropertyName("isModel")]
    bool IsModel,
    [property: JsonPropertyName("url")] string? Url,
    [property: JsonPropertyName("posterUrl")]
    string? PosterUrl,
    [property: JsonPropertyName("videoFallbackUrl")]
    string? VideoFallbackUrl,
    [property: JsonPropertyName("extraInfo")]
    ExtraInfo? ExtraInfo
);

public record ExtraInfo(
    [property: JsonPropertyName("url")] string Url
);

public record SkuDimension(
    [property: JsonPropertyName("dimensionId")]
    string DimensionId,
    [property: JsonPropertyName("value")] double Value,
    [property: JsonPropertyName("dimensionName")]
    string DimensionName
);