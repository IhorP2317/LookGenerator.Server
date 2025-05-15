namespace LookGenerator.Application.Common.Helpers;

public partial class NaturalStringComparer : IComparer<string?>
{
    public int Compare(string? x, string? y)
    {
        switch (x)
        {
            case null when y == null:
                return 0;
            case null:
                return 1;
        }

        if (y == null) return -1;

        return StringComparer.OrdinalIgnoreCase.Compare(
            MyRegex().Replace(x, match => match.Value.PadLeft(10, '0')),
            MyRegex1().Replace(y, match => match.Value.PadLeft(10, '0'))
        );
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"\d+")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
    [System.Text.RegularExpressions.GeneratedRegex(@"\d+")]
    private static partial System.Text.RegularExpressions.Regex MyRegex1();
}
