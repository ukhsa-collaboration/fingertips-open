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
    }
}
