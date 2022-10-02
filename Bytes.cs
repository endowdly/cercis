namespace Cercis;

using static System.Math;

enum UnitPrefix : sbyte
{
    None = 0,
    Kilo = 1,
    Mega = 2,
    Giga = 3,
}

static class Bytes
{
    static UnitPrefix GetPrefix(ulong n) => n switch
    {
        >= 1_000_000_000 => UnitPrefix.Giga,
        >= 1_000_000 => UnitPrefix.Mega,
        >= 1_000 => UnitPrefix.Kilo,
        _ => UnitPrefix.None,
    };

    static double Convert(ulong n, UnitPrefix from, UnitPrefix to)
    { 
        var steps= from - to;

        if (steps == 0)
            return n;

        return steps < 0 ? n * Pow(1000.0, steps) : n * Pow(1000.0, -steps);
    }

    public static string GetDescription(UnitPrefix x)
    {
        string[] fString =
        {
            "B",
            "KB",
            "MB",
            "GB",
        };

        return fString[(int)x];
    }

    public static double ConvertFrom(ulong n) => Convert(n, UnitPrefix.None, GetPrefix(n));

    public static string Prettify(ulong n)
    {
        var x = GetPrefix(n);
        var y = ConvertFrom(n);

        return x == UnitPrefix.None
            ? y.ToString() + Literal.Space + "B"
            : y.ToString("n2") + Literal.Space + GetDescription(x);
    }
}
