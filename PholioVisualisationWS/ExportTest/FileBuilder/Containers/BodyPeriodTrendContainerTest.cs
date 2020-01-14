using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest.FileBuilder.Containers
{
    [TestClass]
    public class BodyPeriodTrendContainerTest
    {
        private MultipleCoreDataCollector _coreDataCollector;
        private CsvBuilderAttributesForPeriodsWrapper _attributesForPeriodsMock;
        private IndicatorMetadata _indicatorMetadata;
        private CoreDataSet _coreData;

        private BodyPeriodTrendContainer _trendContainer;

        [TestInitialize]
        public void Start()
        {
            _coreDataCollector = new MultipleCoreDataCollector();
            _attributesForPeriodsMock = new CsvBuilderAttributesForPeriodsWrapper();

            _indicatorMetadata = new IndicatorMetadata { Unit = new Unit { Value = 1 } };
            _coreData = new CoreDataSet();
            _trendContainer = new BodyPeriodTrendContainer(_coreDataCollector, _attributesForPeriodsMock);
        }

        [TestMethod]
        public void ShouldGetTrendMarkerForNationalSmokeTest()
        {
            var trendMaker = _trendContainer.GetTrendMarker(_indicatorMetadata, _coreData, ExportAreaHelper.GeographicalCategory.National, new Grouping());

            Assert.IsNotNull(trendMaker);
            Assert.IsInstanceOfType(trendMaker, typeof(TrendMarkerResult));
        }

        [TestMethod]
        public void ShouldGetTrendMarkerForSubNationalSmokeTest()
        {
            var trendMaker = _trendContainer.GetTrendMarker(_indicatorMetadata, _coreData, ExportAreaHelper.GeographicalCategory.SubNational, new Grouping());

            Assert.IsNotNull(trendMaker);
            Assert.IsInstanceOfType(trendMaker, typeof(TrendMarkerResult));
        }

        [TestMethod]
        public void ShouldGetTrendMarkerLocalSmokeTest()
        {
            var trendMaker = _trendContainer.GetTrendMarker(_indicatorMetadata, _coreData, ExportAreaHelper.GeographicalCategory.Local, new Grouping());

            Assert.IsNotNull(trendMaker);
            Assert.IsInstanceOfType(trendMaker, typeof(TrendMarkerResult));
        }
    }
}