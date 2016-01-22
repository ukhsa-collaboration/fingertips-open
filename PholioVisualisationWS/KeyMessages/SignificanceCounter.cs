using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public class SignificanceCounter
    {
        public int Red { get; private set; }
        public int Amber { get; private set; }
        public int Green { get; private set; }

        private double totalRedAmberGreen;

        /// <summary>
        /// For Mock object creation
        /// </summary>
        public SignificanceCounter()
        {                
        }

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

            totalRedAmberGreen = (double) (Red + Amber + Green);
        }

        
        public virtual double ProportionGreen
        {
            get { return Green / totalRedAmberGreen; }
            internal set { } // internal for testing
        }

        public virtual double ProportionRed
        {
            get { return Red / totalRedAmberGreen; }
            internal set { } // internal for testing
        }

        public virtual double ProportionAmber
        {
            get { return Amber / totalRedAmberGreen; }
            internal set { } // internal for testing
        }
    }
}
