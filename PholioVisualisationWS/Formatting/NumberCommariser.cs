using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.Formatting
{
    public static class NumberCommariser
    {
        public static string Commarise0DP(double d)
        {
            return string.Format("{0:#,###0}", d);
        }

        public static string Commarise1DP(double d)
        {
            return string.Format("{0:#,###0.0}", d);
        }

        public static string Commarise2DP(double d)
        {
            return string.Format("{0:#,###0.00}", d);
        }

        public static string Commarise3DP(double d)
        {
            return string.Format("{0:#,###0.000}", d);
        }

        public static string CommariseFormattedValue(string s)
        {
            if (s.IndexOf('.') > -1)
            {
                var bits = s.Split('.');
                return Commarise0DP(double.Parse(bits[0])) + '.' + bits[1];
            }
            return Commarise0DP(double.Parse(s));
        }
    }
}
