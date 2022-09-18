namespace Cercis
{
    using System;
    using System.Collections.Generic;

    enum UnitPrefix : sbyte
    {
        None = 0,
        Kilo = 1,
        Mega = 2,
        Giga = 3,
    }

    static class Bytes
    { 
        static UnitPrefix GetPrefix(ulong bytes)
        {
            return bytes > 1000000000
                ? UnitPrefix.Giga
                : bytes > 1000000
                    ? UnitPrefix.Mega
                    : bytes > 1000
                        ? UnitPrefix.Kilo
                        : UnitPrefix.None;
        }

        public static double Convert(ulong n, UnitPrefix from, UnitPrefix to)
        {

            var steps = from - to;

            if (steps == 0)
                return n;

            return steps < 0 
                ? n * Math.Pow(1e3, steps)
                : n / Math.Pow(1e3, steps); 
        }

        public static string GetDescription(this UnitPrefix x)
        {
            string value;
            
            var friendlyStrings = new Dictionary<UnitPrefix, string>
            {
                { UnitPrefix.Kilo, "KB" },
                { UnitPrefix.Mega, "MB" },
                { UnitPrefix.Giga, "GB" },
                { UnitPrefix.None, "B" },
            };

            if (friendlyStrings.TryGetValue(x, out value))
                return value;

            return "?";
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
 
}