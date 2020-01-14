using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AverageCalculatorFactory
    {
        public static AverageCalculator New(IEnumerable<CoreDataSet> dataList, IndicatorMetadata indicatorMetadata)
        {
            // Do not calculate averages if the flag is not set
            // Return a null average calculator object instead
            if (!indicatorMetadata.ShouldAveragesBeCalculated)
            {
                return new NullAverageCalculator();
            }
            
            // Standard average 
            if (IsStandardAverageValid(indicatorMetadata.ValueTypeId))
            {
                return new StandardAverageCalculator(new CoreDataSetFilter(dataList),
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

        public static bool IsStandardAverageValid(int valueTypeId)
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
