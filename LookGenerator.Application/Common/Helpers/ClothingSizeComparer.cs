namespace LookGenerator.Application.Common.Helpers;




public partial class ClothingSizeComparer : IComparer<string?>
{
    // rank for letter sizes…
    private static readonly Dictionary<string,int> SizeRank = new(StringComparer.OrdinalIgnoreCase) {
        {"XXS",10}, {"XS",20}, {"S",30},
        {"M",40}, {"L",50}, {"XL",60}, {"XXL",70}, {"XXXL",80}
    };

    // we’ll use numeric prefix "0" and letter prefix "1"
    private const string NumericPrefix = "0";  
    private const string LetterPrefix  = "1";  

    public int Compare(string? x, string? y)
    {
        if (x == null && y == null) return 0;
        if (x == null)            return -1;
        if (y == null)            return  1;

        var nx = NormalizeSize(x);
        var ny = NormalizeSize(y);
        return StringComparer.OrdinalIgnoreCase.Compare(nx, ny);
    }

    private string NormalizeSize(string size)
    {
        // 1) Pure numeric?
        if (int.TryParse(size, out var n)) {
            // NumericPrefix + zero-padded number
            return NumericPrefix + n.ToString("D5");
        }

        // 2) Simple letter size?
        if (SizeRank.TryGetValue(size, out var rank)) {
            return LetterPrefix + rank.ToString("D5");
        }

        // 3) Hybrids ("10-12", "M-L", etc.)
        if (!size.Contains('-')) return LetterPrefix + "99999" + size;
        var parts = size.Split('-', 2);
        // numeric range?
        if (int.TryParse(parts[0], out var n0)) {
            return NumericPrefix + n0.ToString("D5");
        }
        // letter range?
        if (SizeRank.TryGetValue(parts[0], out var r0) && 
            SizeRank.TryGetValue(parts[1], out var r1)) 
        {
            var avg = (r0 + r1)/2;
            return LetterPrefix + avg.ToString("D5");
        }

        // 4) fallback: push to end of letter bucket
        return LetterPrefix + "99999" + size;
    }
}
