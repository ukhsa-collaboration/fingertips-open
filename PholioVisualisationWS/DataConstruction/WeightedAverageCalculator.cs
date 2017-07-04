using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class WeightedAverageCalculator : AverageCalculator
    {
        private CoreDataSetFilter coreDataSetFilter;
        private Unit unit;

        public WeightedAverageCalculator(CoreDataSetFilter coreDataSetFilter, Unit unit)
        {
            this.coreDataSetFilter = coreDataSetFilter;
            this.unit = unit;
        }

        public override CoreDataSet Average
        {
            get
            {
                var dataList = coreDataSetFilter.SelectWhereCountAndDenominatorAreValid().ToList();

                if (dataList.Any())
                {
                    return CalculateAverage(dataList, unit);
                }

                return null;
            }
        }

        public static CoreDataSet CalculateAverage(IList<CoreDataSet> validData, Unit unit)
        {
            var average = CoreDataSet.GetNullObject();
            average.Count = validData.Sum(x => x.Count.Value);
            average.Denominator = validData.Sum(x => x.Denominator);
            average.Value = (average.Count.Value / average.Denominator) * unit.Value;

            average.ValueNoteId = ValueNoteIds.ValueAggregatedFromAllKnownGeographyValues;
            return average;
        }
    }
}