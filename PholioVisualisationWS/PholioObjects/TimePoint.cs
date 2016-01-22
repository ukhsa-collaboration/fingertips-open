
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.PholioObjects
{
    public class TimePoint
    {
        public const int Undefined = -1;

        public int Year = Undefined;
        public int Quarter = Undefined;
        public int Month = Undefined;

        public bool IsQuarterly
        {
            get { return Quarter > 0; }
        }

        public bool IsMonthly
        {
            get { return Month > 0; }
        }
    }
}
