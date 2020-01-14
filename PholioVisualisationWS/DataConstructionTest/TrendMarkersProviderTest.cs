using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.Analysis.TrendMarkers;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

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
            const string areaCode1 = AreaCodes.CountyUa_Leicestershire;
            const string areaCode2 = AreaCodes.CountyUa_Bexley;

            var areas = new List<IArea>
            {
                new Area { Code = areaCode1},
                new Area { Code = areaCode2}
            };

            var dictionary = new Dictionary<string, IList<CoreDataSet>>();
            dictionary.Add(areaCode1, new List<CoreDataSet>());
            dictionary.Add(areaCode2, new List<CoreDataSet>());
            var mockTrendReader = new Mock<ITrendDataReader>();
            mockTrendReader.Setup(x => x.GetTrendDataForMultipleAreas(It.IsAny<Grouping>(), It.IsAny<string[]>()))
                .Returns(dictionary);

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