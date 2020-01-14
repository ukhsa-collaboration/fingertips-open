
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.PholioObjects
{
    public class QuinaryPopulation
    {
        public int ChildAreaCount { get; set; }
        public IList<QuinaryPopulationValue> Values { get; set; }

        public QuinaryPopulation()
        {
            Values = new List<QuinaryPopulationValue>();
        }

        public static QuinaryPopulation New(IList<CoreDataSet> data)
        {
            var population = new QuinaryPopulation
            {
                Values = data.Select(QuinaryPopulationValue.New).ToList()
            };

            return population;
        }
    }
}
