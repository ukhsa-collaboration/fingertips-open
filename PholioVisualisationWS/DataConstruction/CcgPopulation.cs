using System;
using System.Collections.Generic;

namespace PholioVisualisation.DataConstruction
{
    public class CcgPopulation
    {
        /// <summary>
        /// Key is practice code, Value is the practice population.
        /// Contains the populations of all the practices in the CCG.
        /// </summary>
        public IDictionary<string, double> PracticeCodeToPopulation;

        /// <summary>
        /// Total population for CCG.
        /// </summary>
        public double TotalPopulation { get; set; }

        public string AreaCode { get; set; }
    }
}
