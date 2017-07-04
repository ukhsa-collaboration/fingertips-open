using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class CountAverageCalculator : AverageCalculator
    {
        private CoreDataSetFilter coreDataSetFilter;

        public CountAverageCalculator(CoreDataSetFilter coreDataSetFilter)
        {
            this.coreDataSetFilter = coreDataSetFilter;
        }

        public override CoreDataSet Average
        {
            get
            {
                var dataList = coreDataSetFilter.SelectWhereCountIsValid().ToList();

                if (dataList.Any())
                {
                    return CalculateAverage(dataList);
                }

                return null;
            }
        }

        public static CoreDataSet CalculateAverage(IList<CoreDataSet> validData)
        {
            var average = CoreDataSet.GetNullObject();
            average.Count = validData.Sum(x => x.Count);
            average.Value = average.Count.Value;
            return average;
        }
    }
}
