using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Constants;

public static class LookGenerationConstants
{
    public const double FeetTolerance = 0.5;
    public const double BodyTolerance = 2.0;
    public const double BeltBodyTolerance = 5.0;
    public static readonly Dictionary<string, Dictionary<string, string[]>> RequiredMeasurementsByCategoryAndGender = new()
    {
      {
        "MEN", new Dictionary<string, string[]>
        {
          { "TOP", ["chestContour", "hipContour"] },
          { "BOTTOM", ["waistContour", "hipContour"] },
          { "SHOES", ["footMeasure"] }
        }
      },
      {
        "WOMEN", new Dictionary<string, string[]>
        {
          { "TOP", ["chestContour", "waistContour", "hipContour"] },
          { "BOTTOM", ["waistContour", "hipContour"] },
          { "SHOES", ["footMeasure"] }
        }
      }
    };
    public static readonly Dictionary<string, ProductBodyZone> SizeZonesByCategory = new()
    {
      { "TOP", ProductBodyZone.UpperBody },
      { "BOTTOM", ProductBodyZone.LowerBody },
      { "SHOES", ProductBodyZone.Feet }
    };


    public const string OutputFormat = """
                                       ## Output Format (STRICT):
                                       {{
                                         "looks": [
                                           {{
                                             "colorPalette": "Monochromatic",
                                             "lookName": "Example Look",
                                             "productsItems": [
                                              "29cbe7e7-c6c5-4420-90e3-2bceee919fed",
                                              "5a98131b-1906-4571-a644-e6a6558a1c47",
                                              "c608f222-45c2-4a69-bd72-9b52fab6ec2f"
                                             ]
                                           }}
                                         ],
                                         "description": "Optional description of the look"
                                       }} 
                                       """;

    public static readonly OutputResultJsonScheme OutputResultJson = new(
        Name: "LookGenerationResponse",
        Description: "Structured response containing generated looks using valid productItem IDs and color palettes.",
        Format: @"
    {
      ""type"": ""object"",
      ""properties"": {
        ""looks"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""colorPalette"": {
                ""type"": ""string"",
                ""description"": ""Color palette category for this look, e.g., Monochromatic, Analogous, Triadic, Split-complementary, Tetradic.""
              },
              ""lookName"": {
                ""type"": ""string"",
                ""description"": ""Descriptive name for the fashion look.""
              },
              ""productsItems"": {
                ""type"": ""array"",
                ""items"": {
                  ""type"": ""string"",
                  ""description"": ""Product item ID that is included in this look.""
                },
                ""description"": ""List of product item IDs included in this look.""
              },
              ""description"": {
                ""type"": [""string"", ""null""],
                ""description"": ""Optional per-look description (e.g. null or a brief note)."",
                ""nullable"": true
              }
            },
            ""required"": [""colorPalette"", ""lookName"", ""productsItems"", ""description""],
            ""additionalProperties"": false
          }
        },
        ""description"": {
          ""type"": [""string"", ""null""],
          ""description"": ""Overall response description. Null if at least one look generated, otherwise a brief failure reason."",
          ""nullable"": true
        }
      },
      ""required"": [""looks"", ""description""],
      ""additionalProperties"": false
    }
    "
    );
  
}