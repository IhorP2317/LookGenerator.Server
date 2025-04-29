using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.Looks;

public record LookPhotos(
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("per_page")] int PerPage,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("next")] bool Next,
    [property: JsonPropertyName("data")] List<PhotoData> Data,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("status")] int Status
);
    






