using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AverageCalculatorFactory
    {
        public static AverageCalculator New(IEnumerable<CoreDataSet> dataList, IndicatorMetadata indicatorMetadata)
        {
            // Weighted average 
            if (IsWeightedAverageValid(indicatorMetadata.ValueTypeId))
            {
                return new WeightedAverageCalculator(new CoreDataSetFilter(dataList),
                    indicatorMetadata.Unit);
            }

            switch (indicatorMetadata.ValueTypeId)
            {
                case ValueTypeId.Count:
                    return new CountAverageCalculator(new CoreDataSetFilter(dataList));

                default:
                    return new NullAverageCalculator();
            }
        }

        public static bool IsWeightedAverageValid(int valueTypeId)
        {
            switch (valueTypeId)
            {
                case ValueTypeId.IndirectlyStandardisedRatio:
                case ValueTypeId.CrudeRate:
                case ValueTypeId.Proportion:
                    return true;

                default:
                    return false;
            }
        }
    }
}
