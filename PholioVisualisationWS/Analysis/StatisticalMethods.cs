
using System;

namespace PholioVisualisation.Analysis
{
    public class StatisticalMethods
    {
        public static Double PoisLow(double vz, double p)
        {
            double v = 0.5;
            double dv = 0.5;
            while (dv > 0.0000001)
            {
                dv = dv / 2;
                if (PoisP((1 + vz) * v / (1 - v), vz, 10000000000) > p)
                    v = v - dv;
                else
                    v = v + dv;
            }
            return (1 + vz) * v / (1 - v);
        }

        public static Double PoisHigh(double vz, double p)
        {
            double v = 0.5;
            double dv = 0.5;
            while (dv > 0.0000001)
            {
                dv = dv / 2;
                if (PoisP((1 + vz) * v / (1 - v), 0, vz) < p)
                    v = v - dv;
                else
                    v = v + dv;
            }
            return (1 + vz) * v / (1 - v);
        }

        private static Double PoisP(double z, double x1, double x2)
        {
            double q = 1;
            double tot = 0;
            double s = 0;
            double k = 0;
            while (k <= z || q > (tot * 0.0000000001))
            {
                tot = tot + q;
                if (k >= x1 && k <= x2)
                    s = s + q;
                if (tot > 1E+30)
                {
                    s = s / 1E+30;
                    tot = tot / 1E+30;
                    q = q / 1E+30;
                }
                k = k + 1;
                q = q * z / k;
            }
            return s / tot;
        }
    }
}
