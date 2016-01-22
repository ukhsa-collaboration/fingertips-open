
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.PholioObjects
{
    public class TimePeriod : TimePoint
    {
        public int YearRange = Undefined;

        public TimePoint GetRollingMonthlyEndPoint()
        {
            int yearRange = YearRange;
            if (yearRange <= 0)
            {
                yearRange = 1;
            }

            int year = Year + yearRange;
            if (Month == 1)
            {
                year--;
            }

            return new TimePoint { Year = year, Month = GetTwelthMonthFrom() };
        }

        public TimePoint GetRollingQuarterlyEndPoint()
        {
            int year = Year + YearRange;
            if (Quarter == 1)
            {
                year--;
            }

            return new TimePoint { Year = year, Quarter = GetForthQuarterFrom() };
        }

        private int GetForthQuarterFrom()
        {
            int newQuarter = Quarter;
            if (Quarter > 0)
            {
                if (Quarter == 1)
                {
                    newQuarter = 4;
                }
                else
                {
                    newQuarter = Quarter - 1;
                }
            }
            return newQuarter;
        }


        private int GetTwelthMonthFrom()
        {
            int newMonth = Month;
            if (Month != -1)
            {
                if (Month == 1)
                {
                    newMonth = 12;
                }
                else
                {
                    newMonth = Month - 1;
                }
            }
            return newMonth;
        }

        public TimePeriod GetTimePeriodForYearBefore()
        {
            return new TimePeriod
            {
                Year = Year - 1,
                YearRange = YearRange,
                Quarter = Quarter,
                Month = Month
            };
        }

        public static TimePeriod GetBaseline(Grouping grouping)
        {
            return new TimePeriod
            {
                Year = grouping.BaselineYear,
                YearRange = grouping.YearRange,
                Quarter = grouping.BaselineQuarter,
                Month = grouping.BaselineMonth
            };
        }

        public static TimePeriod GetDataPoint(Grouping grouping)
        {
            return new TimePeriod
            {
                Year = grouping.DataPointYear,
                YearRange = grouping.YearRange,
                Quarter = grouping.DataPointQuarter,
                Month = grouping.DataPointMonth
            };
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}", Year, YearRange, Quarter, Month);
        }
    }
}
