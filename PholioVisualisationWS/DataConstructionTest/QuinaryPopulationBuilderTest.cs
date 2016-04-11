
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
        public const int SupportingGroupId = GroupIds.PracticeProfiles_SupportingIndicators;

        [TestMethod]
        public void TestPractice()
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                AreaCode = AreaCodes.Gp_AdamHouseSandiacre,
                DataPointOffset = 0,
                GroupId = SupportingGroupId
            };
            builder.Build();
            CheckPopulationValues(builder.Values);
            Assert.IsNotNull(builder.EthnicityText);
        }

        [TestMethod]
        public void TestCcg()
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                AreaCode = AreaCodes.Ccg_CambridgeshirePeterborough,
                DataPointOffset = 0,
                GroupId = SupportingGroupId
            };
            builder.Build();
            CheckPopulationValues(builder.Values);
            Assert.IsNull(builder.EthnicityText);
        }

        [TestMethod]
        public void TestEngland()
        {
            QuinaryPopulationBuilder builder = new QuinaryPopulationBuilder
            {
                AreaCode = AreaCodes.England,
                DataPointOffset = 0,
                GroupId = SupportingGroupId
            };
            builder.Build();
            CheckPopulationValues(builder.Values);
            Assert.IsNull(builder.EthnicityText);
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
