using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

                if (dataList.Any() && IsValueAsExpected(dataList.First()))
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

        private bool IsValueAsExpected(CoreDataSet data)
        {
            var average = (data.Count.Value / data.Denominator) * unit.Value;
            return Round(data.Value).Equals(Round(average));
        }

        private static double Round(double d)
        {
            return Math.Round(d, 0, MidpointRounding.AwayFromZero);
        }
    }
}