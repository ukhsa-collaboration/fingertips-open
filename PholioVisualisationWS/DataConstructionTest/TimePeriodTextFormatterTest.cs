using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class TimePeriodTextFormatterTest
    {
        [TestMethod]
        public void TestFormat()
        {
            var builder = new TimePeriodTextFormatter(
                new IndicatorMetadata { YearTypeId = (int)YearTypeIds.Calendar});
            var periodString = builder.Format(new TimePeriod { Year = 2000, YearRange = 1 });
            Assert.AreEqual("2000", periodString);
        }
    }
}
