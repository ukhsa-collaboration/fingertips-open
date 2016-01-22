using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class PracticeAxis
    {
        /// <summary>
        /// Key is area code, value is CoreDataSet.Value
        /// </summary>
        public Dictionary<string, float> IndicatorData = new Dictionary<string, float>();

        public Limits Limits;

        public string Title;
    }
}
