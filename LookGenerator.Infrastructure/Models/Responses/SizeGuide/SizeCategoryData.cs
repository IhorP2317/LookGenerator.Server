using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.SizeGuide;

public class SizeCategoryData
{
    [JsonPropertyName("familyIds")]
    public List<int> FamilyIds { get; set; } = default!;
    [JsonPropertyName("BERSHKA_MAN")]
    public GenderSizeData BERSHKA_MAN { get; set; } = default!;
    [JsonPropertyName("BERSHKA_WOMAN")]
    public GenderSizeData BERSHKA_WOMAN { get; set; } = default!;
}