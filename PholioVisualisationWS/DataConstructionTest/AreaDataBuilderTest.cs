using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaDataBuilderTest
    {
        [TestMethod]
        public void TestLatestDataOnly()
        {
            AreaDataBuilder builder = new AreaDataBuilder
            {
                Areas = new List<IArea> { new Area { Code = AreaCodes.England } },
                ComparatorAreaCodes = new List<string> { AreaCodes.England },
                GroupId = GroupIds.PracticeProfiles_PracticeSummary,
                AreaTypeId = AreaTypeIds.GpPractice,
                IncludeTimePeriods = true,
                LatestDataOnly = true
            };

            var map = builder.Build();

            foreach (var key in map.Keys)
            {
                var fullAreaDataList = map[key];
                foreach (var areaData in fullAreaDataList)
                {
                    var fullAreaData = areaData as FullAreaData;

                    Assert.AreEqual(1, fullAreaData.Data.Count);
                    Assert.AreEqual(1, fullAreaData.Periods.Count);
                }
            }
        }

        [TestMethod]
        public void TestTimePeriodsAreIncludedForMissingData()
        {
            AreaDataBuilder builder = new AreaDataBuilder
            {
                Areas = new List<IArea> { new Area { Code = AreaCodes.England } },
                ComparatorAreaCodes = new List<string> { AreaCodes.England },
                GroupId = GroupIds.PracticeProfiles_PracticeSummary,
                AreaTypeId = AreaTypeIds.GpPractice,
                IncludeTimePeriods = true
            };

            var map = builder.Build();

            foreach (var key in map.Keys)
            {
                var fullAreaDataList = map[key];
                foreach (var areaData in fullAreaDataList)
                {
                    var fullAreaData = areaData as FullAreaData;

                    Assert.AreEqual(fullAreaData.Data.Count, fullAreaData.Periods.Count,
                        "Number of data point and time periods should be equal but are not for indicator ID " + fullAreaData.IndicatorId);   
                }
            }
        }

        [TestMethod]
        public void TestRegressionTestForPracticeProfiles()
        {
            AreaDataBuilder builder = new AreaDataBuilder
                {
                    Areas = Area(),
                    ComparatorAreaCodes = new List<string> { AreaCodes.England },
                    GroupId = GroupIds.PracticeProfiles_SecondaryCareUse,
                    AreaTypeId = AreaTypeIds.GpPractice
                };

            var map = builder.Build();
            Assert.AreEqual(1, map.Count);
        }

        [TestMethod]
        public void TestDiabetesPracticeDetails()
        {
            var areaCode = AreaCodes.Gp_PracticeInBarnetCcg;
            var parentAreaCode = AreaCodes.Ccg_Barnet;

            AreaDataBuilder builder = new AreaDataBuilder
            {
                Areas = new List<IArea>{new Area{Code = areaCode}},
                ComparatorAreaCodes = new List<string> { parentAreaCode },
                GroupId = GroupIds.Diabetes_PrevalenceAndRisk,
                AreaTypeId = AreaTypeIds.GpPractice
            };

            var dataList = builder.Build()[areaCode];
            foreach (var simpleAreaData in dataList)
            {
                Assert.IsNotNull(simpleAreaData);
                Assert.IsTrue(simpleAreaData.Significances[parentAreaCode].First()/*only one time period*/ > 0);
            }
        }

        private static List<IArea> Area()
        {
            return new List<IArea> { new Area { Code = AreaCodes.Pct_Ashton } };
        }
    }

}
