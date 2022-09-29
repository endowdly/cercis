namespace Cercis;

enum UnitPrefix : sbyte
{
    None = 0,
    Kilo = 1,
    Mega = 2,
    Giga = 3,
}

static class Bytes
{
    static UnitPrefix GetPrefix(ulong n)
    {
        return n > 1000000000
            ? UnitPrefix.Giga
            : n > 1000000
                ? UnitPrefix.Mega
                : n > 1000
                    ? UnitPrefix.Kilo
                    : UnitPrefix.None;
    }

    static double Convert(ulong n, UnitPrefix from, UnitPrefix to)
    {
        var steps = from - to;

        if (steps == 0)
            return n;

        return steps < 0 ? n * Math.Pow(1e3, steps) : n / Math.Pow(1e3, steps);
    }

    public static string GetDescription(this UnitPrefix x)
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

    public static double ConvertFrom(ulong n)
    {
        return Convert(n, UnitPrefix.None, GetPrefix(n));
    }

    public static string Prettify(ulong n)
    {
        var x = GetPrefix(n);
        var y = ConvertFrom(n);

        return x == UnitPrefix.None
            ? y.ToString() + Literal.Space + x.GetDescription()
            : y.ToString("n2") + Literal.Space + x.GetDescription();
    }
}
