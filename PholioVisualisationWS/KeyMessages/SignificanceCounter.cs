using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{

    public interface ISignificanceCounter
    {
        int Red { get; set; }
        int Amber { get; set; }
        int Green { get; set; }
        double GetProportionGreen();
        double GetProportionAmber();
        double GetProportionRed();
    }

    public class SignificanceCounter : ISignificanceCounter
    {
        public int Red { get; set; }
        public int Amber { get; set; }
        public int Green { get; set; }

        private readonly double _totalRedAmberGreen;

        public SignificanceCounter(IEnumerable<Significance> significances)
        {
            foreach (var significance in significances)
            {
                switch (significance)
                {
                    case Significance.Better:
                        Green++;
                        break;
                    case Significance.Worse:
                        Red++;
                        break;
                    case Significance.Same:
                        Amber++;
                        break;
                    case Significance.None:
                        // Do nothing
                        break;
                }
            }

            _totalRedAmberGreen = Red + Amber + Green;
        }

        public double GetProportionGreen()
        {
            return Green / _totalRedAmberGreen;
        }

        public double GetProportionRed()
        {
            return Red / _totalRedAmberGreen;
        }

        public double GetProportionAmber()
        {
            return Amber / _totalRedAmberGreen;
        }
    }
}
