
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootTrendBuilderYearly : GroupRootTrendBuilderBase
    {
        protected override CoreDataSet GetDataAtSpecificTimePeriod(IList<CoreDataSet> dataList, TimePeriod period)
        {
            return (from d in dataList
                                    where d.Year == period.Year
                                    select d).FirstOrDefault();
        }
    }
}