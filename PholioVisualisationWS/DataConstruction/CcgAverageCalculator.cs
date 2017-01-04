using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class CcgAverageCalculator
    {
        public CoreDataSet Average { get; set; }

        public CcgAverageCalculator(IList<CoreDataSet> dataList, CcgPopulation ccgPopulation, IndicatorMetadata indicatorMetadata)
        {
            Unit unit = indicatorMetadata.Unit;

            if (AverageCalculatorFactory.IsWeightedAverageValid(indicatorMetadata.ValueTypeId) == false)
            {
                return;
            }

            if (dataList.Any(x => x.IsCountValid && x.IsDenominatorValid))
            {
                // Weighted average
                var validData = new CoreDataSetFilter(dataList).SelectWhereCountAndDenominatorAreValid().ToList();

                if (RuleAreEnoughPracticeValuesByPopulation.Validates(validData, ccgPopulation))
                {
                    Average = WeightedAverageCalculator.CalculateAverage(validData, unit);
                    Average.AreaCode = ccgPopulation.AreaCode;
                }
            }
            else if (dataList.Any(x => x.IsValueValid))
            {
                // Population weighted average
                var practiceCodeToPopulationMap = ccgPopulation.PracticeCodeToPopulation;

                // Keep only data where population is available
                var dataWithPopulation = dataList.Where(x => practiceCodeToPopulationMap.ContainsKey(x.AreaCode)).ToList();

                if (RuleAreEnoughPracticeValuesByPopulation.Validates(dataWithPopulation, ccgPopulation))
                {
                    Average = CoreDataSet.GetNullObject(ccgPopulation.AreaCode);

                    // Population weighted average
                    Average.Value = dataWithPopulation.Sum(data => (data.Value * practiceCodeToPopulationMap[data.AreaCode])) /
                              ccgPopulation.TotalPopulation;
                }
            }
        }
    }
}
