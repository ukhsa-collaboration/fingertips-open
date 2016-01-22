using System.Collections.Generic;
using System.Linq;  
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// Rule that validates if a list of data values represents at least 90%
    /// of a CCG's population.
    /// </summary>
    public static class RuleAreEnoughPracticeValuesByPopulation
    {
        /// <summary>
        /// Whether or not there is enough data available to cover 
        /// </summary>
        public static bool Validates(IEnumerable<CoreDataSet> data, CcgPopulation ccgPopulation)
        {
            // Practices that have values
            var areaCodesWithValues = data.Select(x => x.AreaCode);

            // Population represented by indicator values
            double populationRepresentingAvailableData = ccgPopulation.PracticeCodeToPopulation
                .Where(x => areaCodesWithValues.Contains(x.Key))
                .Sum(x => x.Value);

            // Does the data available cover enough of the population
            return populationRepresentingAvailableData >= ccgPopulation.TotalPopulation * 0.9;
        }
    }
}
