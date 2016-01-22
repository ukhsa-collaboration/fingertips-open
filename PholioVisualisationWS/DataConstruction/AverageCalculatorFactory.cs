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
            switch (indicatorMetadata.ValueTypeId)
            {
                case ValueTypeId.IndirectlyStandardisedRatio:
                case ValueTypeId.CrudeRate:
                case ValueTypeId.Proportion:
                    return new WeightedAverageCalculator(new CoreDataSetFilter(dataList),
                        indicatorMetadata.Unit);

                case ValueTypeId.Count:
                    return new CountAverageCalculator(new CoreDataSetFilter(dataList));

                default:
                    return new NullAverageCalculator();

            }
        }
    }
}
