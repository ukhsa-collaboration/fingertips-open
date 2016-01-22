using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class BenchmarkDataProviderTest
    {
        [TestMethod]
        public void TestGetBenchmarkData_DataReadFromDatabaseIsReturned()
        {
            var mockReader = new Mock<GroupDataReader>(MockBehavior.Strict);

            mockReader.Setup(x => x.GetCoreData(It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), It.IsAny<string>()))
                .Returns(new List<CoreDataSet> { new CoreDataSet { Value = 1 } });

            var provider = new BenchmarkDataProvider(mockReader.Object);

            var data = provider.GetBenchmarkData(new Grouping(), new TimePeriod(),
                new Mock<AverageCalculator>().Object, new Area());

            Assert.IsTrue(data.IsValueValid);

            mockReader.Verify();
        }

        [TestMethod]
        public void TestGetBenchmarkData_WhenNoDataInDatabaseAndNoAverageThenNullCoreDataSetIsReturned()
        {
            var mockReader = new Mock<GroupDataReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.GetCoreData(It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), It.IsAny<string>()))
                .Returns(new List<CoreDataSet>());

            var averageCalculator = new Mock<AverageCalculator>();
            averageCalculator.SetupGet(x => x.Average)
                .Returns(default(CoreDataSet)/*i.e. null*/);

            var data = new BenchmarkDataProvider(mockReader.Object)
                .GetBenchmarkData(new Grouping(), new TimePeriod(), averageCalculator.Object, new Area());

            Assert.IsFalse(data.IsValueValid);

            mockReader.Verify();
        }

        [TestMethod]
        public void TestGetBenchmarkData_WhenNoDataInDatabaseAverageIsReturned()
        {
            var mockReader = new Mock<GroupDataReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.GetCoreData(It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), It.IsAny<string>()))
                .Returns(new List<CoreDataSet>());

            var averageCalculator = new Mock<AverageCalculator>();
            averageCalculator.SetupGet(x => x.Average)
                .Returns(new CoreDataSet { Value = 1 });

            var data = new BenchmarkDataProvider(mockReader.Object)
                .GetBenchmarkData(new Grouping(), new TimePeriod(), averageCalculator.Object, new Area());

            Assert.IsTrue(data.IsValueValid);

            mockReader.Verify();
        }

        [TestMethod]
        public void TestGetBenchmarkData_WhenAreaIsCategoryArea()
        {
            var mockReader = new Mock<GroupDataReader>(MockBehavior.Strict);

            mockReader.Setup(x => x.GetCoreDataForCategoryArea(It.IsAny<Grouping>(),
                    It.IsAny<TimePeriod>(), It.IsAny<CategoryArea>()))
                .Returns(new CoreDataSet { Value = 1 });

            var provider = new BenchmarkDataProvider(mockReader.Object);

            var parentArea = new Mock<CategoryArea>(MockBehavior.Strict);
            parentArea.Setup(x => x.Code).Returns("code");

            var data = provider.GetBenchmarkData(new Grouping(), new TimePeriod(),
                new Mock<AverageCalculator>().Object, parentArea.Object);

            Assert.IsTrue(data.IsValueValid);
            Assert.IsNotNull("code", data.AreaCode, "AreaCode has not been set");

            mockReader.Verify();
        }

        public BenchmarkDataProvider DataProvider()
        {
            return new BenchmarkDataProvider(ReaderFactory.GetGroupDataReader());
        }
    }
}

