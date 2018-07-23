using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.SearchQuerying;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class IndicatorSearchActionTest
    {
        private IndicatorSearchAction _indicatorSearchAction;

        [TestInitialize]
        public void TestInitialize()
        {
            // Init dependencies
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var profileReader = ReaderFactory.GetProfileReader();
            var areaTypeListProvider =
                new AreaTypeListProvider(new GroupIdProvider(profileReader), ReaderFactory.GetAreasReader(), groupDataReader);
            var groupingListProvider = new GroupingListProvider(groupDataReader, profileReader);
            var indicatorFilter = new IndicatorKnowledgeFilter(new IndicatorMetadataRepository());
            var indicatorsWithDataByAreaTypeBuilder = new IndicatorsWithDataByAreaTypeBuilder(groupDataReader, groupingListProvider);

            _indicatorSearchAction = new IndicatorSearchAction(areaTypeListProvider,
                groupDataReader, indicatorsWithDataByAreaTypeBuilder, indicatorFilter);
        }

        [TestMethod]
        public void TestGetAreaTypeIdToIndicatorIdsWithData()
        {
           var areaTypeIdToIndicatorIds = _indicatorSearchAction
                .GetAreaTypeIdToIndicatorIdsWithData("smoking", new List<int> {ProfileIds.Phof});

            // Assert: expected area type
            var areaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
            Assert.IsTrue(areaTypeIdToIndicatorIds.ContainsKey(areaTypeId));

            // Assert: expected indicator ID for area type
            Assert.IsTrue(areaTypeIdToIndicatorIds[areaTypeId].Contains(IndicatorIds.SmokingAtTimeOfDelivery));
        }
    }
}

