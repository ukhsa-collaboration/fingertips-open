
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootTrendBuilderQuarterly : GroupRootTrendBuilderBase
    {
        protected override CoreDataSet GetDataAtSpecificTimePeriod(IList<CoreDataSet> dataList, TimePeriod period)
        {
            return (from d in dataList
                                    where d.Year == period.Year && d.Quarter == period.Quarter
                                    select d).FirstOrDefault();
        }
    }
}