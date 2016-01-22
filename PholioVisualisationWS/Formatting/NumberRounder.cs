using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.Formatting
{
    public class NumberRounder
    {
        public static double ToNearest10(double amount)
        {
            return DoRounding(amount, 10);
        }

        public static double ToNearest100(double amount)
        {
            return DoRounding(amount, 100);
        }

        public static double ToNearest1000(double amount)
        {
            return DoRounding(amount, 1000);
        }

        private static double DoRounding(double amount, double roundTo)
        {
            return (Math.Round(amount/roundTo, MidpointRounding.AwayFromZero)) * roundTo;
        }
    }
}
