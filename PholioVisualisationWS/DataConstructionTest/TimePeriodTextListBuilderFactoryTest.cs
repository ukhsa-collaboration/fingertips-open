using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class TimePeriodTextListBuilderFactoryTest
    {
        [TestMethod]
        public void TestNewTrueReturnsTimePeriodTextListBuilder()
        {
            Assert.IsTrue(
                TimePeriodTextListBuilderFactory.New(true, new IndicatorMetadata())
                is TimePeriodTextListBuilder);
        }

        [TestMethod]
        public void TestNewFalseReturnsNullBuilder()
        {
            Assert.IsTrue(
                TimePeriodTextListBuilderFactory.New(false, new IndicatorMetadata())
                is NullTimePeriodTextListBuilder);
        }
    }
}
