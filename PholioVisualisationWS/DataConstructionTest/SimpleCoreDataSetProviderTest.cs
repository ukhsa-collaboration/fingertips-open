using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class SimpleCoreDataSetProviderTest
    {
        private string areaCode = "a";
        private TimePeriod timePeriod = new TimePeriod();
        private int indicatorId = 1;
        private int sexId = 2;
        private int ageId = 3;

        [TestMethod]
        public void TestGetCoreDataReturnsNullIfNoDataInDatabase()
        {
            var mock = new Moq.Mock<GroupDataReader>();
            var grouping = Grouping();

            mock.Setup(x => x
                .GetCoreData(grouping, timePeriod, areaCode))
                .Returns(new List<CoreDataSet>());

            SimpleCoreDataSetProvider provider = new SimpleCoreDataSetProvider(Area(), mock.Object);
            Assert.IsNull(provider.GetData(grouping, timePeriod, null/*metadata not used*/));
        }

        [TestMethod]
        public void TestGetCoreDataReturnsCoreDataSetIfMatchingRowInDatabase()
        {
            double val = 3;
            var mock = new Moq.Mock<GroupDataReader>();
            var grouping = Grouping();

            mock.Setup(x => x
                .GetCoreData(grouping, timePeriod, areaCode))
                .Returns(new List<CoreDataSet> { new CoreDataSet { Value = val } });

            SimpleCoreDataSetProvider provider = new SimpleCoreDataSetProvider(Area(), mock.Object);
            var data = provider.GetData(grouping, timePeriod, null/*metadata not used*/);
            Assert.AreEqual(val, data.Value);
        }

        private Grouping Grouping()
        {
            Grouping grouping = new Grouping
            {
                IndicatorId = indicatorId,
                SexId = sexId,
                AgeId = ageId
            };
            return grouping;
        }

        private Area Area()
        {
            Area area = new Area { Code = areaCode };
            return area;
        }
    }
}
