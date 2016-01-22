
using System.Collections.Generic;

namespace PholioVisualisation.PholioObjects
{
    public class TimePeriodIterator
    {
        private const int QuartersInOneYear = 4;

        private TimePeriod start;
        private TimePeriod end;

        private List<TimePeriod> points = new List<TimePeriod>();

        public TimePeriodIterator(TimePeriod start, TimePeriod end, YearType yearType)
        {
            this.start = start;
            this.end = end;

            if (start.IsQuarterly)
            {
                InitQuarterlyTimePeriods(yearType);
            }
            else if (start.IsMonthly)
            {
                InitMonthlyTimePeriods();
            }
            else
            {
                InitYearlyTimePeriods();
            }
        }

        /// <summary>
        /// Time periods in ascending order (e.g. 2007 first .... 2010 last). 
        /// </summary>
        public IList<TimePeriod> TimePeriods
        {
            get { return points; }
        }

        private void InitMonthlyTimePeriods()
        {
            int range = start.YearRange;
            int startMonth = start.Month;
            for (int year = start.Year; year <= end.Year; year++)
            {
                for (int month = startMonth; month <= 12; month++)
                {
                    if (year == end.Year && month > end.Month)
                    {
                        break;
                    }

                    TimePeriod period = new TimePeriod { Year = year, YearRange = range, Month = month };
                    points.Add(period);
                }

                startMonth = 1;
            }
        }

        private void InitQuarterlyTimePeriods(YearType yearType)
        {
            int range = start.YearRange;
            var maxQuarter = GetMaximumNumberOfQuarters(yearType, range);
            int startQuarter = start.Quarter;
            for (int year = start.Year; year <= end.Year; year++)
            {
                for (int quarter = startQuarter; quarter <= maxQuarter; quarter++)
                {
                    if (year == end.Year && quarter > end.Quarter)
                    {
                        break;
                    }

                    TimePeriod period = new TimePeriod { Year = year, YearRange = range, Quarter = quarter };
                    points.Add(period);
                }

                startQuarter = 1;
            }

        }

        private static int GetMaximumNumberOfQuarters(YearType yearType, int range)
        {
            if (yearType.Id == YearTypeIds.FinancialMultiYearCumulativeQuarter)
            {
                return QuartersInOneYear * range;
            }
            return QuartersInOneYear;
        }

        private void InitYearlyTimePeriods()
        {
            int range = start.YearRange;
            for (int year = start.Year; year <= end.Year; year++)
            {
                TimePeriod period = new TimePeriod { Year = year, YearRange = range };
                points.Add(period);
            }
        }

    }
}
