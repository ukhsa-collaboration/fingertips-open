using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace DataAccessTest
{
    [TestClass]
    public class PholioIntegrityTest
    {
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        [TestMethod]
        public void TestAllPossibleChildAreaTypesHaveDefaultParentOptionsDefined()
        {
            var defaultChildAreaTypeIds = GetChildAreaTypeIdsThatHaveParentOptionDefined(
                ProfileIds.Undefined);

            var profiles = ReaderFactory.GetProfileReader().GetAllProfiles();
            foreach (var profileConfig in profiles)
            {
                var profileChildAreaTypeIds =
                    GetChildAreaTypeIdsThatHaveParentOptionDefined(profileConfig.ProfileId);

                foreach (var profileChildAreaTypeId in profileChildAreaTypeIds)
                {
                    Assert.IsTrue(defaultChildAreaTypeIds.Contains(profileChildAreaTypeId),
                        "Default parent area type option not defined for child area type Id:" +
                        profileChildAreaTypeId + ". You need to add row to WS_ProfileParentAreaOptions");
                }
            }
        }

        [TestMethod]
        public void TestAllChildAreaTypesThatHaveGroupingsAlsoHaveDefaultParentOptionsDefined()
        {
            var defaultChildAreaTypeIds = GetChildAreaTypeIdsThatHaveParentOptionDefined(
                ProfileIds.Undefined);
                
            var areaTypeIdsOnGroupings = ReaderFactory.GetGroupDataReader()
                .GetDistinctGroupingAreaTypeIdsForAllProfiles();

            foreach (var areaTypeId in areaTypeIdsOnGroupings)
            {
                Assert.IsTrue(defaultChildAreaTypeIds.Contains(areaTypeId),
                    "Default parent area type option not defined for child area type Id:" +
                    areaTypeId + ". You need to add row to WS_ProfileParentAreaOptions");
            }
        }

        private List<int> GetChildAreaTypeIdsThatHaveParentOptionDefined(int profileId)
        {
            var defaultChildAreaTypeIds = areasReader.GetParentAreaGroups(profileId)
                .Select(x => x.ChildAreaTypeId)
                .Distinct()
                .ToList();
            return defaultChildAreaTypeIds;
        }
    }
}
