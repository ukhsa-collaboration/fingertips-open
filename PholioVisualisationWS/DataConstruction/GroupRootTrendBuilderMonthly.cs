
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootTrendBuilderMonthly : GroupRootTrendBuilderBase
    {
        protected override CoreDataSet GetDataAtSpecificTimePeriod(IList<CoreDataSet> dataList, TimePeriod period)
        {
            return (from d in dataList
                                    where d.Year == period.Year && d.Month == period.Month 
                                    select d).FirstOrDefault();
        }

        protected override TrendDataPoint GetTrendDataAtSpecificTimePeriod(IList<TrendDataPoint> dataList, TimePeriod period)
        {
            return (from d in dataList
                where d.CoreDataSet.Year == period.Year && d.CoreDataSet.Month == period.Month
                select d).FirstOrDefault();
        }
    }
}