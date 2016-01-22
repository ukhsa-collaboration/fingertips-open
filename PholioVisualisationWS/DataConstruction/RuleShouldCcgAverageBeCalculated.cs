using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class RuleShouldCcgAverageBeCalculated
    {

        public static bool Validates(Grouping grouping, IEnumerable<CoreDataSet> data, CcgPopulation ccgPopulation)
        {
            return RuleAreEnoughPracticeValuesByPopulation.Validates(data, ccgPopulation) &&
                   RuleShouldCcgAverageBeCalculatedForGroup.Validates(grouping);
        }

    }
}