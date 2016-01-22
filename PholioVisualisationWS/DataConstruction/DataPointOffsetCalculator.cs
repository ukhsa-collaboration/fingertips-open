
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class DataPointOffsetCalculator
    {
        public const int InvalidYear = 1900;

        public TimePeriod TimePeriod { get; set; }

        public DataPointOffsetCalculator(Grouping grouping, int dataPointOffset, YearType yearType)
        {
            TimePeriod dataPointPeriod = TimePeriod.GetDataPoint(grouping);
            if (dataPointOffset == 0)
            {
                TimePeriod = TimePeriod.GetDataPoint(grouping);
            }
            else
            {
                SetOffsetTimePeriod(grouping, dataPointOffset, dataPointPeriod, yearType);
            }
        }

        private void SetOffsetTimePeriod(Grouping grouping, int dataPointOffset, TimePeriod dataPointPeriod, YearType yearType)
        {
            if (dataPointOffset < 0)
            {
                // Ensures datapoint cannot be exceeded
                TimePeriod = dataPointPeriod;
            }
            else
            {
                TimePeriod baseline = TimePeriod.GetBaseline(grouping);
                TimePeriodIterator iterator = new TimePeriodIterator(baseline, dataPointPeriod, yearType);
                int periodCount = iterator.TimePeriods.Count;
                int index = periodCount - 1 - dataPointOffset;
                if (index < periodCount && index > 0)
                {
                    TimePeriod = iterator.TimePeriods[index];
                }
                else
                {
                    if (index < 0)
                    {
                        // Ensures baseline cannot be overshot
                        TimePeriod = new TimePeriod { Year = InvalidYear };
                    }
                    else
                    {
                        TimePeriod = baseline;
                    }
                }
            }
        }
    }
}
