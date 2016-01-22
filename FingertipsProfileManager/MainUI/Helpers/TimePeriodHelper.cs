using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.Upload;

namespace Fpm.MainUI.Helpers
{
    public class TimePeriodHelper
    {
        private TimePeriodReader timePeriodReader = new TimePeriodReader();
        private GroupingPlusName grouping;
        private TimePeriod baseline;
        private TimePeriod datapoint;
        private TimePeriod latestPeriod;

        public TimePeriodHelper(GroupingPlusName grouping)
        {
            this.grouping = grouping;
            this.baseline = TimePeriod.GetBaseline(grouping);
            this.datapoint = TimePeriod.GetDataPoint(grouping);
        }

        public string GetDatapointString()
        {
            return timePeriodReader.GetPeriodString(datapoint, grouping.YearTypeId);
        }

        public string GetBaselineString()
        {
            return timePeriodReader.GetPeriodString(baseline, grouping.YearTypeId);
        }

        public string GetLatestPeriodString()
        {
            return timePeriodReader.GetPeriodString(latestPeriod, grouping.YearTypeId);
        }

        public TimePeriod GetPeriodIfLaterThanDatapoint()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var periods = reader.GetCoreDataSetTimePeriods(grouping);

            if (periods.Any())
            {
                IEnumerable<TimePeriod> relevantPeriods;
                if (datapoint.IsMonthly)
                {
                    relevantPeriods = periods.Where(x => x.Month != TimePeriod.Undefined);
                } else if (datapoint.IsQuarterly)
                {
                    relevantPeriods = periods.Where(x => x.Quarter != TimePeriod.Undefined);
                } else
                {
                    // Annual
                    relevantPeriods = periods.Where(x => 
                        x.Month == TimePeriod.Undefined &&
                        x.Quarter == TimePeriod.Undefined );
                }

                if (relevantPeriods.Any())
                {
                    var last = relevantPeriods.Last();
                    if (last.IsLaterThan(datapoint))
                    {
                        latestPeriod = last;
                        return last;
                    }
                }
            }

            return null;
        }
    }
}