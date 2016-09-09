using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class TrendMarkersProviderTest
    {
        public const string AreaCode = AreaCodes.England;

        [TestMethod]
        public void Test_Trend_Result_Provided_For_Every_Area()
        {
            // Arrange
            var areaCode1 = AreaCodes.CountyUa_Leicestershire;
            var areaCode2 = AreaCodes.CountyUa_Bexley;

            var mockTrendReader = new Mock<ITrendDataReader>();
            mockTrendReader.Setup(x => x.GetTrendData(It.IsAny<Grouping>(), It.IsAny<string>()))
                .Returns(new List<CoreDataSet> { });

            var areas = new List<IArea>
            {
                new Area { Code = areaCode1},
                new Area { Code = areaCode2}
            };

            var indicatorMetadata = new IndicatorMetadata
            {
                Unit = new Unit { Value = 1 },
                ValueTypeId = ValueTypeId.CrudeRate,
            };

            // Act
            var trendMarkers = new TrendMarkersProvider(mockTrendReader.Object, new TrendMarkerCalculator())
                .GetTrendResults(areas, indicatorMetadata, new Grouping());

            // Assert
            Assert.AreEqual(2, trendMarkers.Count);
            Assert.AreEqual(trendMarkers[areaCode1].Marker, TrendMarker.CannotBeCalculated);
            Assert.AreEqual(trendMarkers[areaCode2].Marker, TrendMarker.CannotBeCalculated);
        }
    }
}
