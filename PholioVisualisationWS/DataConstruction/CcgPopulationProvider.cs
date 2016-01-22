using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataConstruction
{
    public class CcgPopulationProvider
    {
        private Dictionary<string, CcgPopulation> ccgPopulations = new Dictionary<string, CcgPopulation>();
        private PholioReader pholioReader;

        /// <summary>
        /// Parameterless constructor required for mocking.
        /// </summary>
        protected CcgPopulationProvider() { }

        public CcgPopulationProvider(PholioReader pholioReader)
        {
            this.pholioReader = pholioReader;
        }

        public virtual CcgPopulation GetPopulation(string areaCode)
        {
            if (ccgPopulations.ContainsKey(areaCode))
            {
                return ccgPopulations[areaCode];
            }

            var population = NewPopulation(areaCode);
            ccgPopulations[areaCode] = population;
            return population;
        }

        private CcgPopulation NewPopulation(string areaCode)
        {
            var populations = pholioReader.GetCcgPracticePopulations(areaCode);
            var population = new CcgPopulation
            {
                AreaCode = areaCode,
                PracticeCodeToPopulation = populations,
                TotalPopulation = populations.Values.Sum()
            };
            return population;
        }
    }
}