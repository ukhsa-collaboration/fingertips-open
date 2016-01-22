using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class CategoryAreaCoreDataSetProviderTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            var categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority, 1);
            var provider = new CategoryAreaCoreDataSetProvider(categoryArea, ReaderFactory.GetGroupDataReader());
            Assert.IsNotNull(provider);
        }

        [TestMethod]
        public void TestGetData()
        {
            var categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority, 1);
            var grouping = new Grouping();
            var timePeriod = new TimePeriod();

            var reader = new Mock<GroupDataReader>(MockBehavior.Strict);
            reader.Setup(x => x.GetCoreDataForCategoryArea(grouping, timePeriod, categoryArea))
                .Returns(new CoreDataSet());

            var provider = new CategoryAreaCoreDataSetProvider(categoryArea, reader.Object);

            Assert.IsNotNull(provider.GetData(grouping, timePeriod, null));

            reader.VerifyAll();
        }
    }
}
