
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class QuinaryPopulationBuilderTest
    {
        public const int PopulationGroupId = GroupIds.Population;

        [TestMethod]
        public void TestCcgPopulation()
        {
            var areaTypeId = AreaTypeIds.CcgsPostApr2018;

             QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                AreaCode = AreaCodes.Ccg_AireDaleWharfdaleAndCraven,
                DataPointOffset = 0,
                GroupId = PopulationGroupId,
                AreaTypeId = areaTypeId
            };
            builder.BuildPopulationTotalsAndPercentages(AreaCodes.Ccg_AireDaleWharfdaleAndCraven);
            CheckPopulationValues(builder.PopulationPercentages);
        }

        [TestMethod]
        public void TestEnglandPopulation()
        {
            var areaTypeId = AreaTypeIds.CcgsPostApr2018;

            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                AreaCode = AreaCodes.England,
                DataPointOffset = 0,
                GroupId = PopulationGroupId,
                AreaTypeId = areaTypeId
            };
            builder.BuildPopulationTotalsAndPercentages(AreaCodes.England);
            CheckPopulationValues(builder.PopulationPercentages);
        }

        [TestMethod]
        public void TestPracticePopulation()
        {
            var areaCode = AreaCodes.Gp_MeersbrookSheffield;
            var areaTypeId = AreaTypeIds.GpPractice;

            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                AreaCode = areaCode,
                DataPointOffset = 0,
                GroupId = PopulationGroupId,
                AreaTypeId = areaTypeId
            };
            builder.BuildPopulationTotalsAndPercentages(areaCode);
            CheckPopulationValues(builder.PopulationPercentages);
        }

        private static void CheckPopulationValues(Dictionary<int, IEnumerable<double>> values)
        {
            foreach (var sexId in new[] { SexIds.Male, SexIds.Female })
            {
                IList<double> sexValues = values[sexId].ToList();

                Assert.AreEqual(20, sexValues.Count);
                foreach (var value in sexValues)
                {
                    Assert.IsTrue(value > 0);
                }
            }
        }
    }
}
