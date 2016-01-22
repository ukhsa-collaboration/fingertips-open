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
    public class ShapeCoreDataSetProviderTest
    {
        private const string AreaCode = "a";
        private Area area = new Area { Code = AreaCode };

        [TestMethod]
        public void GetData()
        {
            double val = 2;
            Grouping grouping = new Grouping();
            TimePeriod timePeriod = new TimePeriod();

            var data = PracticeAggregateCoreDataSetProvider(grouping, timePeriod, val)
                .GetData(grouping, timePeriod, IndicatorMetadata());
            Assert.AreEqual(val, data.Value);
        }

        [TestMethod]
        public void GetDataReturnsNullIfValueUndefined()
        {
            Grouping grouping = new Grouping();
            TimePeriod timePeriod = new TimePeriod();

            var data = PracticeAggregateCoreDataSetProvider(grouping, timePeriod, ValueData.NullValue)
                .GetData(grouping, timePeriod, IndicatorMetadata());
            Assert.IsNull(data);
        }

        private ShapeCoreDataSetProvider PracticeAggregateCoreDataSetProvider(Grouping grouping,
            TimePeriod timePeriod, double val)
        {
            var practiceDataAccess = new Mock<PracticeDataAccess>(MockBehavior.Strict);
            practiceDataAccess.Protected();
            practiceDataAccess.Setup(x => x
                .GetPracticeAggregateDataValue(grouping, timePeriod, AreaCode))
                .Returns(val);

            return new ShapeCoreDataSetProvider(area, practiceDataAccess.Object);
        }

        private IndicatorMetadata IndicatorMetadata()
        {
            // Never used
            return null;
        }
    }
}
