using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class BenchmarkDataProviderTest
    {
        private Mock<GroupDataReader> _groupDataReader;
        private Mock<AverageCalculator> _averageCalculator;

        [TestInitialize]
        public void TestInitialize()
        {
            _groupDataReader = new Mock<GroupDataReader>(MockBehavior.Strict);
            _averageCalculator = new Mock<AverageCalculator>();
        }

        [TestMethod]
        public void TestGetBenchmarkData_DataReadFromDatabaseIsReturned()
        {
            // Data is read from database
            _groupDataReader.Setup(x => x.GetCoreData(It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), It.IsAny<string>()))
                .Returns(new List<CoreDataSet> { new CoreDataSet { Value = 1 } });

            // Act
            var data = DataProvider().GetBenchmarkData(new Grouping(), new TimePeriod(),
                _averageCalculator.Object, new Area());

            // Assert
            Assert.IsTrue(data.IsValueValid);
            _groupDataReader.Verify();
        }

        [TestMethod]
        public void TestGetBenchmarkData_WhenNoDataInDatabaseAndNoAverageThenNullCoreDataSetIsReturned()
        {
            // Data is not found in database
            _groupDataReader.Setup(x => x.GetCoreData(It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), It.IsAny<string>()))
                .Returns(new List<CoreDataSet>());

            // Average cannot be calculated
            _averageCalculator.SetupGet(x => x.Average)
                .Returns(default(CoreDataSet)/*i.e. null*/);

            // Act
            var data = DataProvider()
                .GetBenchmarkData(new Grouping(), new TimePeriod(), _averageCalculator.Object, new Area());

            // Assert
            Assert.IsFalse(data.IsValueValid);
            _groupDataReader.Verify();
        }

        [TestMethod]
        public void TestGetBenchmarkData_WhenNoDataInDatabaseAverageIsReturned()
        {
            // No data found in database
            _groupDataReader.Setup(x => x.GetCoreData(It.IsAny<Grouping>(), It.IsAny<TimePeriod>(), It.IsAny<string>()))
                .Returns(new List<CoreDataSet>());

            // Average is calculated
            _averageCalculator.SetupGet(x => x.Average)
                .Returns(new CoreDataSet { Value = 1 });

            // Act
            var data = DataProvider()
                .GetBenchmarkData(new Grouping(), new TimePeriod(), _averageCalculator.Object, new Area());

            // Assert
            Assert.IsTrue(data.IsValueValid);
            _groupDataReader.Verify();
        }

        [TestMethod]
        public void TestGetBenchmarkData_WhenAreaIsCategoryAreaAndDataIsRetrievedFromDatabase()
        {
            // Category area found in database
            _groupDataReader.Setup(x => x.GetCoreDataForCategoryArea(It.IsAny<Grouping>(),
                    It.IsAny<TimePeriod>(), It.IsAny<ICategoryArea>()))
                .Returns(new CoreDataSet { Value = 1 });

            // Area setup
            var parentArea = new Mock<ICategoryArea>(MockBehavior.Strict);
            parentArea.Setup(x => x.Code).Returns("code");

            // Act
            var data = DataProvider().GetBenchmarkData(new Grouping(), new TimePeriod(),
                _averageCalculator.Object, parentArea.Object);

            // Assert
            Assert.IsTrue(data.IsValueValid);
            _groupDataReader.Verify();
        }

        [TestMethod]
        public void TestGetBenchmarkData_WhenAreaIsCategoryAreaAndDataIsCalculated()
        {
            // No data in database
            _groupDataReader.Setup(x => x.GetCoreDataForCategoryArea(It.IsAny<Grouping>(),
                    It.IsAny<TimePeriod>(), It.IsAny<ICategoryArea>()))
                .Returns((CoreDataSet)null);

            // Average is calculated
            _averageCalculator.SetupGet(x => x.Average)
                .Returns(new CoreDataSet { Value = 1 });

            // Category area properties are copied to data
            var parentArea = new Mock<ICategoryArea>(MockBehavior.Strict);
            parentArea.Setup(x => x.ParentAreaCode).Returns("parent-code");
            parentArea.Setup(x => x.Code).Returns("code");
            parentArea.Setup(x => x.CategoryId).Returns(1);
            parentArea.Setup(x => x.CategoryTypeId).Returns(2);

            // Act
            var data = DataProvider().GetBenchmarkData(new Grouping(), new TimePeriod(),
                _averageCalculator.Object, parentArea.Object);

            // Assert
            Assert.IsTrue(data.IsValueValid);
            Assert.AreEqual("parent-code", data.AreaCode, "AreaCode has not been set");
            _groupDataReader.Verify();
        }

        public BenchmarkDataProvider DataProvider()
        {
            return new BenchmarkDataProvider(_groupDataReader.Object);
        }
    }
}

