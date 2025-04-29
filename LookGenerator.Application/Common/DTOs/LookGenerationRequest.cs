using System.Text.Json.Serialization;

namespace LookGenerator.Application.Common.DTOs
{
    public record LookGenerationRequest(
        [property: JsonPropertyName("products")]
        ICollection<ProductRequest> Products);




    public record ProductRequest(
        [property: JsonPropertyName("id")]
        Guid Id, 
        [property: JsonPropertyName("name")]
        string Name, 
        [property: JsonPropertyName("bodyZone")]
        string BodyZone,
        [property: JsonPropertyName("categories")]
        ICollection<string> Categories,
        [property: JsonPropertyName("productItems")]
        ICollection<ProductItemRequest> ProductItems ,
        [property: JsonPropertyName("description")]
        string? Description = null);
    

    public record ProductItemRequest(
        [property: JsonPropertyName("id")]
        Guid Id, 
        [property: JsonPropertyName("colour")]
        ColourRequest Colour);

    public record ColourRequest(
        [property: JsonPropertyName("id")]
        int Id, 
        [property: JsonPropertyName("name")]
        string Name);
}