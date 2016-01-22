using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class NullTimePeriodTextListBuilderTest
    {
        [TestMethod]
        public void TestToListReturnsNull()
        {
            var builder = new NullTimePeriodTextListBuilder();
            builder.Add(new TimePeriod());
            Assert.IsNull(builder.ToList());
        }
    }
}
