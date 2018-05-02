using System;

namespace Shicho
{
    internal static class Util
    {
        static readonly string[] ByteSuffixes = {
            "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
        };

        public static string ToByteUnits(Int64 value, int decimalPlaces = 1, int maxMag = 4)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + ToByteUnits(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = Math.Min((int)Math.Log(value, 1024), maxMag - 1);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000) {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format(
                "{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                ByteSuffixes[mag]
            );
        }
    }
}
