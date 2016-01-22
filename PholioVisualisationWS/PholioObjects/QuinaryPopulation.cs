
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.PholioObjects
{
    public class QuinaryPopulation
    {
        public int ChildAreaCount { get; set; }
        public List<QuinaryPopulationValue> Values { get; set; }

        public QuinaryPopulation()
        {
            Values = new List<QuinaryPopulationValue>();
        }
    }
}
