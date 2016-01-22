
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class CoreDataTimeFilter
    {
        private IEnumerable<CoreDataSet> data;

        public CoreDataTimeFilter(IEnumerable<CoreDataSet> data)
        {
            this.data = data;
        }

        public IEnumerable<CoreDataSet> Filter(TimePeriod start, TimePeriod end)
        {
            int startY = start.Year;
            int endY = end.Year;

            if (start.IsQuarterly)
            {
                int startQ = start.Quarter;
                int endQ = end.Quarter;

                return (from d in data
                        where (d.Year >= startY && d.Year <= endY) &&
                            // In end year but after end quarter
                    !(d.Year == endY && d.Quarter > endQ) &&
                            // In start year but after start quarter
                    !(d.Year == startY && d.Quarter < startQ)
                        select d).ToList();
            }

            if (start.IsMonthly)
            {
                int startM = start.Month;
                int endM = end.Month;

                return (from d in data
                        where (d.Year >= startY && d.Year <= endY) &&
                            // In end year but after end month
                        !(d.Year == endY && d.Month > endM) &&
                            // In start year but after start month
                        !(d.Year == startY && d.Month < startM)
                        select d).ToList();
            }

            // Yearly data
            return (from d in data
                    where
                        (d.Year >= startY && d.Year <= endY) &&
                        d.YearRange == start.YearRange
                    select d).ToList();
        }
    }
}
