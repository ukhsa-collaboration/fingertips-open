using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class CcgCoreDataSetProviderTest
    {
        private string areaCode = "a";
        private double practiceValue = 10;

        [TestMethod]
        public void TestCheckDatabaseForValueBeforeCalculating()
        {
            double valueInDatabase = 2;

            Area area = Area;
            var timePeriod = new TimePeriod();
            var grouping = new Grouping();

            var groupDataReader = new Mock<GroupDataReader>(MockBehavior.Strict);
            groupDataReader.Protected();
            groupDataReader.Setup(x => x
                .GetCoreData(grouping, timePeriod, areaCode))
                .Returns(new List<CoreDataSet> { new CoreDataSet { Value = valueInDatabase } });

            CcgCoreDataSetProvider provider = new CcgCoreDataSetProvider(area, null, null, groupDataReader.Object);
            var coreDataSet = provider.GetData(grouping, timePeriod, null);

            Assert.AreEqual(valueInDatabase, coreDataSet.Value);
        }

        [TestMethod]
        public void TestValidData()
        {
            Area area = Area;
            var timePeriod = new TimePeriod();
            var grouping = new Grouping();

            CcgCoreDataSetProvider provider = new CcgCoreDataSetProvider(area,
                ValidPopulationProvider(),
                ValidDataListProvider(grouping, area, timePeriod),
                EmptyGroupDataReader(grouping, timePeriod));
            var coreDataSet = provider.GetData(grouping, timePeriod, new IndicatorMetadata());

            Assert.AreEqual(practiceValue, coreDataSet.Value);
        }

        [TestMethod]
        public void TestGetDataReturnsNullIfNoChildPracticeData()
        {
            Area area = Area;
            var timePeriod = new TimePeriod();
            var grouping = new Grouping();

            CcgCoreDataSetProvider provider = new CcgCoreDataSetProvider(area,
                ValidPopulationProvider(),
                EmptyDataListProvider(grouping, area, timePeriod),
                EmptyGroupDataReader(grouping, timePeriod));
            var coreDataSet = provider.GetData(grouping, timePeriod, new IndicatorMetadata());

            Assert.IsNull(coreDataSet);
        }

        [TestMethod]
        public void TestGetDataReturnsNullIfInvalidGroupId()
        {
            var area = Area;
            var timePeriod = new TimePeriod();
            var grouping = new Grouping { GroupId = RuleShouldCcgAverageBeCalculatedForGroup.InvalidId };

            CcgCoreDataSetProvider provider = new CcgCoreDataSetProvider(area,
                ValidPopulationProvider(),
                ValidDataListProvider(grouping, area, timePeriod),
                EmptyGroupDataReader(grouping, timePeriod));
            var coreDataSet = provider.GetData(grouping, timePeriod, new IndicatorMetadata());

            Assert.IsNull(coreDataSet);
        }

        private Area Area
        {
            get
            {
                Area area = new Area { Code = areaCode };
                return area;
            }
        }

        private GroupDataReader EmptyGroupDataReader(Grouping grouping, TimePeriod timePeriod)
        {
            var groupDataReader = new Mock<GroupDataReader>(MockBehavior.Strict);
            groupDataReader.Protected();
            groupDataReader.Setup(x => x
                .GetCoreData(grouping, timePeriod, areaCode))
                .Returns(new List<CoreDataSet>());
            return groupDataReader.Object;
        }

        private CcgPopulationProvider ValidPopulationProvider()
        {
            var ccgPopulation = new CcgPopulation
            {
                AreaCode = areaCode,
                TotalPopulation = 100,
                PracticeCodeToPopulation = new Dictionary<string, double> {
                    {"a", 50},
                    {"b", 50}
                }
            };
            var populationProvider = new Mock<CcgPopulationProvider>(MockBehavior.Strict);
            populationProvider.Protected();
            populationProvider.Setup(x => x
                .GetPopulation(areaCode))
                .Returns(ccgPopulation);
            return populationProvider.Object;
        }

        private CoreDataSetListProvider ValidDataListProvider(Grouping grouping, Area area, TimePeriod timePeriod)
        {
            var childPracticeDataList = new List<CoreDataSet>
            {
                new CoreDataSet {AreaCode = "a", Value = practiceValue},
                new CoreDataSet {AreaCode = "b", Value = practiceValue}
            };

            return DataListProvider(grouping, area, timePeriod, childPracticeDataList);
        }

        private static CoreDataSetListProvider DataListProvider(Grouping grouping, Area area, TimePeriod timePeriod, List<CoreDataSet> childPracticeDataList)
        {
            var dataListProvider = new Mock<CoreDataSetListProvider>(MockBehavior.Strict);
            dataListProvider.Protected();
            dataListProvider.Setup(x => x
                .GetChildAreaData(grouping, area, timePeriod))
                .Returns(childPracticeDataList);
            return dataListProvider.Object;
        }

        private static CoreDataSetListProvider EmptyDataListProvider(Grouping grouping, Area area, TimePeriod timePeriod)
        {
            return DataListProvider(grouping, area, timePeriod, new List<CoreDataSet>());
        }
    }
}
