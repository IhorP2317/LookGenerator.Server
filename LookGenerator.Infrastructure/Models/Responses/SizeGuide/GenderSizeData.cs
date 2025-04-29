using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.SizeGuide;
public class GenderSizeData
{
    [JsonPropertyName("imagesData")]
    public string ImagesData { get; set; } = default!;
    [JsonPropertyName("imageInfo")]
    public List<ImageInfo> ImageInfo { get; set; } = default!;
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;
    [JsonPropertyName("blocks")]
    public List<SizeBlock> Blocks { get; set; } = default!;
}