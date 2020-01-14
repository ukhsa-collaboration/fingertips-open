
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.PholioObjects
{
    public class QuinaryPopulationValue
    {
        public int AgeId;
        public double Value;

        public static QuinaryPopulationValue New(CoreDataSet data)
        {
            return new QuinaryPopulationValue { Value = data.Value, AgeId = data.AgeId };
        }
    }
}
