using Fpm.ProfileData;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.MainUI.Helpers
{
    public class TimePeriodHelper : ITimePeriodHelper
    {
        private readonly TimePeriodReader _timePeriodReader = new TimePeriodReader();
        private readonly GroupingPlusName _grouping;
        private readonly TimePeriod _baseline;
        private readonly TimePeriod _datapoint;
        private TimePeriod _latestPeriod;

        public TimePeriodHelper(GroupingPlusName grouping)
        {
            this._grouping = grouping;
            this._baseline = TimePeriod.GetBaseline(grouping);
            this._datapoint = TimePeriod.GetDataPoint(grouping);
        }

        public string GetDatapointString()
        {
            return _timePeriodReader.GetPeriodString(_datapoint, _grouping.YearTypeId);
        }

        public string GetBaselineString()
        {
            return _timePeriodReader.GetPeriodString(_baseline, _grouping.YearTypeId);
        }

        public string GetLatestPeriodString()
        {
            return _timePeriodReader.GetPeriodString(_latestPeriod, _grouping.YearTypeId);
        }

        public TimePeriod GetPeriodIfLaterThanDatapoint()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var periods = reader.GetCoreDataSetTimePeriods(_grouping);

            if (periods.Any())
            {
                IEnumerable<TimePeriod> relevantPeriods;
                if (_datapoint.IsMonthly)
                {
                    relevantPeriods = periods.Where(x => x.Month != TimePeriod.Undefined);
                } else if (_datapoint.IsQuarterly)
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
                    if (last.IsLaterThan(_datapoint))
                    {
                        _latestPeriod = last;
                        return last;
                    }
                }
            }

            return null;
        }
    }
}