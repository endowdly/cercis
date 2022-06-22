namespace Cercis
{
    using System;
    using System.Collections.Generic;

    enum UnitPrefix : sbyte
    {
        None = 3,
        Kilo = 2,
        Mega = 1,
        Giga = 0,
    }

    static class Bytes
    { 
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
            {
                return value;
            }

            return "?";
        }

        public static UnitPrefix PrettyUnit(ulong bytes)
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
            double result;
            double nSteps;

            nSteps = from - to;

            if (nSteps == 0)
                return n;

            result = n;

            if (nSteps > 0)
            {
                result /= Math.Pow(1000.0, nSteps);
            }
            else
            {
                result *= Math.Pow(1000.0, Math.Abs(nSteps));
            }

            return result;
        }
    } 
}