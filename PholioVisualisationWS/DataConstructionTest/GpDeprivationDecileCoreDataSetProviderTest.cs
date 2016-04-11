using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GpDeprivationDecileCoreDataSetProviderTest
    {
        private const string AreaCode = "cat-1-1";
        private readonly CategoryArea categoryArea = new CategoryArea(AreaCode);

        [TestMethod]
        public void GetData()
        {
            double val = 2;
            var grouping = new Grouping();
            var timePeriod = new TimePeriod();

            CoreDataSet data = CoreDataSetProvider(grouping, timePeriod, val)
                .GetData(grouping, timePeriod, null);
            Assert.AreEqual(val, data.Value);
        }

        [TestMethod]
        public void GetDataReturnsNullIfValueUndefined()
        {
            Grouping grouping = new Grouping();
            TimePeriod timePeriod = new TimePeriod();

            var data = CoreDataSetProvider(grouping, timePeriod, ValueData.NullValue)
                .GetData(grouping, timePeriod, null);
            Assert.IsNull(data);
        }

        private GpDeprivationDecileCoreDataSetProvider CoreDataSetProvider(Grouping grouping,
            TimePeriod timePeriod, double val)
        {
            var practiceDataAccess = new Mock<PracticeDataAccess>(MockBehavior.Strict);
            practiceDataAccess.Protected();
            practiceDataAccess.Setup(x => x
                .GetGpDeprivationDecileDataValue(grouping, timePeriod, categoryArea))
                .Returns(val);

            return new GpDeprivationDecileCoreDataSetProvider(categoryArea, practiceDataAccess.Object);
        }
    }
}