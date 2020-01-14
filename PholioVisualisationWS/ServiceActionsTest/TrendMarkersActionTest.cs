using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class TrendMarkersActionTest
    {
        private Mock<IFilteredChildAreaListProvider> _areaListProvider;
        private Mock<SingleGroupingProvider> _singleGroupingProvider;
        private Mock<ITrendMarkersProvider> _trendMarkersProvider;

        /// <summary>
        /// Define mocks
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _areaListProvider = new Mock<IFilteredChildAreaListProvider>(MockBehavior.Strict);

            _singleGroupingProvider = new Mock<SingleGroupingProvider>(MockBehavior.Strict);

            _trendMarkersProvider = new Mock<ITrendMarkersProvider>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_Trend_Markers_Calculated()
        {
            // Constants and objects
            const int profileId = ProfileIds.Phof;
            const string parentAreaCode = AreaCodes.England;
            const int groupId = GroupIds.Phof_WiderDeterminantsOfHealth;
            const int areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;
            const int indicatorId = IndicatorIds.ChildrenInLowIncomeFamilies;
            const int sexId = SexIds.Persons;
            const int ageId = AgeIds.Under16;
            const string areaCode = AreaCodes.CountyUa_Leicestershire;

            var areas = new List<IArea>
            {
                new Area {Code = areaCode}
            };
            var grouping = new Grouping();

            // Arrange
            _areaListProvider.Setup(x => x.ReadChildAreas(parentAreaCode, profileId, areaTypeId)).Returns(areas);

            _singleGroupingProvider.Setup(x => x.GetGroupingByGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(groupId, areaTypeId, indicatorId, sexId, ageId))
                .Returns(grouping);

            _trendMarkersProvider.Setup(x => x.GetTrendResults(areas, It.IsAny<IndicatorMetadata>(), grouping))
                .Returns(new Dictionary<string, TrendMarkerResult>
                {
                    {areaCode,new TrendMarkerResult()}
                });


            // Act
            var trendMarkers = new TrendMarkersAction(_areaListProvider.Object, _trendMarkersProvider.Object, _singleGroupingProvider.Object)
                .GetTrendMarkers(parentAreaCode, profileId, groupId, areaTypeId, indicatorId, sexId, ageId);

            // Assert
            Assert.AreEqual(1, trendMarkers.Count);

            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _areaListProvider.VerifyAll();
            _singleGroupingProvider.VerifyAll();
            _areaListProvider.VerifyAll();
        }
    }
}
