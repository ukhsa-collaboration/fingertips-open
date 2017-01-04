using System;
using System.Collections.Generic;
using System.Linq;
using FingertipsDataExtractionTool.AverageCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FingertipsDataExtractionToolTest.AverageCalculator
{
    [TestClass]
    public class CalculatorGroupingListProviderTest
    {
        private Mock<IGroupDataReader> _groupDataReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _groupDataReader = new Mock<IGroupDataReader>();
            _groupDataReader.Setup(x => x.GetAllIndicators()).Returns(new List<int> {1});
        }

        [TestMethod]
        public void When_No_Groupings_Found()
        {
            // Arrange
            _groupDataReader.Setup(x => x.GetGroupingsByIndicatorId(1)).Returns(new List<Grouping>());

            // Act
            var groupings = new CalculatorGroupingListProvider(_groupDataReader.Object)
                .GetGroupings();

            // Assert
            Assert.IsFalse(groupings.Any());
            _groupDataReader.VerifyAll();
        }

        [TestMethod]
        public void When_Multiple_Groupings_Found_Then_One_Returned_With_Widest_Date_Range()
        {
            // Arrange
            _groupDataReader.Setup(x => x.GetGroupingsByIndicatorId(1)).Returns(new List<Grouping>
            {
                new Grouping { BaselineYear = 2000, DataPointYear = 2001},
                new Grouping { BaselineYear = 2005, DataPointYear = 2006},
                new Grouping { BaselineYear = 2002, DataPointYear = 2003}
            });

            // Act
            var groupings = new CalculatorGroupingListProvider(_groupDataReader.Object)
                .GetGroupings();

            // Assert
            Assert.AreEqual(1, groupings.Count);
            Assert.AreEqual(2000, groupings.First().BaselineYear);
            Assert.AreEqual(2006, groupings.First().DataPointYear);
            _groupDataReader.VerifyAll();
        }
    }
}
