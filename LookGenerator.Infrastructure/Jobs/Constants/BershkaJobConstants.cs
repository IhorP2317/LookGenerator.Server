using System.Collections.Immutable;

namespace LookGenerator.Infrastructure.Jobs.Constants;

public static class BershkaJobConstants
{
    public static ImmutableDictionary<string, string> ColorMappings =>
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["WHITE"] = "White",
            ["Off white"] = "White",
            ["White / Black"] = "White",
            ["Light blue"] = "Blue",
            ["Dark blue"] = "Blue",
            ["Washed out blue"] = "Blue",
            ["Navy"] = "Blue",
            ["Blue"] = "Blue",
            ["BLACK"] = "Black",
            ["Black"] = "Black",
            ["Cream"] = "Beige",
            ["Sand"] = "Beige",
            ["Taupe"] = "Beige",
            ["Stone"] = "Beige",
            ["Beige"] = "Beige",
            ["Ecru"] = "Beige",
            ["SAND"] = "Beige",
            ["Grey"] = "Grey",
            ["Dark grey"] = "Grey",
            ["Silver"] = "Grey",
            ["SILVER"] = "Grey",
            ["Light Brown"] = "Brown",
            ["Camel"] = "Brown",
            ["Brown"] = "Brown",
            ["Leather"] = "Brown",
            ["Mink"] = "Brown",
            ["Tan"] = "Brown",
            ["Red"] = "Red",
            ["BURGUNDY"] = "Red",
            ["Maroon"] = "Red",
            ["PINK"] = "Pink",
            ["Pink"] = "Pink",
            ["Fuchsia"] = "Pink",
            ["Yellow"] = "Yellow",
            ["Gold"] = "Yellow",
            ["Mustard"] = "Yellow",
            ["Orange"] = "Orange",
            ["Green"] = "Green",
            ["Khaki"] = "Green",
            ["Lime"] = "Green",
            ["Turquoise"] = "Green",
            ["Violet"] = "Violet",
            ["Multicolored"] = "Multicolor",
            ["Multicolor"] = "Multicolor",
            ["Transparent"] = "Transparent"
        }.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
    

}