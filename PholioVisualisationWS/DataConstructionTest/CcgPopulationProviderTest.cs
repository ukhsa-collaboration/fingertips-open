using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class CcgPopulationProviderTest
    {
        private string ccgAreaCode = "c";
        private Dictionary<string, double> practicePopulations = new Dictionary<string, double>
            {
                {"a", 1},
                {"b", 2}
            };

        [TestMethod]
        public void TestCcgPopulationCalculatedCorrectly()
        {
            var population = PopulationProvider().GetPopulation(ccgAreaCode);
            Assert.AreEqual(3, population.TotalPopulation);
            Assert.AreEqual(ccgAreaCode, population.AreaCode);
            Assert.AreEqual(practicePopulations.Count, population.PracticeCodeToPopulation.Count);
        }

        [TestMethod]
        public void TestCcgPopulationsAreCached()
        {
            var mock = MockPholioReader();
            var provider = new CcgPopulationProvider(mock.Object);

            var population = provider.GetPopulation(ccgAreaCode);
            population = provider.GetPopulation(ccgAreaCode);

            mock.Verify(x => x.GetCcgPracticePopulations(ccgAreaCode), Times.Once());
        }

        private CcgPopulationProvider PopulationProvider()
        {
            return new CcgPopulationProvider(MockPholioReader().Object);
        }

        private Mock<PholioReader> MockPholioReader()
        {
            var mock = new Mock<PholioReader>();
            mock.Setup(x => x
                .GetCcgPracticePopulations(ccgAreaCode))
                .Returns(practicePopulations);
            return mock;
        }
    }
}
