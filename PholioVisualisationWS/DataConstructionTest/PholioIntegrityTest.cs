using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class PholioIntegrityTest
    {
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        /// <summary>
        /// All longer lives profiles need the comparator set
        /// to "National and Subnational" in FPM. The exception is GP area type.
        /// </summary>
        [TestMethod]
        public void TestAllLongerLivesProfilesHaveRequiredGroupings()
        {
            var failMessages = new List<string>();

            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var profileIds = ReaderFactory.GetProfileReader().GetLongerLivesProfileIds();
            foreach (var profileId in profileIds)
            {
                var groupIds = groupDataReader.GetGroupIdsOfProfile(profileId);
                foreach (var groupId in groupIds)
                {
                    var groupings = groupDataReader.GetGroupingsByGroupId(groupId)
                        .Where(x => x.AreaTypeId != AreaTypeIds.GpPractice);

                    // Group according to the parameters that are used to display
                    var groupedGrouping = groupings.GroupBy(x =>
                        new { x.IndicatorId, x.AreaTypeId, x.SexId, x.AgeId });

                    foreach (var grouped in groupedGrouping)
                    {
                        if (grouped.Count() == 1)
                        {
                            failMessages.Add(string.Format(
                                 "Comparator must be set to 'National and Subnational' for Indicator ID {0} Profile ID {1}",
                                 grouped.Key.IndicatorId, profileId));
                        }
                    }
                }   
            }

            // Check whether any indicators required the comparator changing
            if (failMessages.Any())
            {
                foreach (var failMessage in failMessages)
                {
                    Console.WriteLine(failMessage);
                }
                Assert.Fail("Comparator must be set to 'National and Subnational' for all Longer Lives indicators");
            }
        }

        [TestMethod]
        public void Test_CIs_Are_Not_Both_Minus_One_Instead_Of_Null()
        {
            var count = ReaderFactory.GetGroupDataReader()
                .GetCoreDataCountWhereBothCI95AreMinusOne();
            Assert.AreEqual(0, count, @"There should be no CoreDataSet rows where both CI95 values are -1. 
FUS should have changed these to nulls instead. Run this script to fix:
update [CoreDataSet]
set  [LowerCI95] = null, [UpperCI95] = null
where [LowerCI95] = -1 and [UpperCI95] = -1
             ");
        }

        [TestMethod]
        public void TestNoDuplicatedIndicatorMetadataOverrides()
        {
            var multiplyOverriddenIndicatorProfilePairs = ReaderFactory.GetPholioReader()
                 .GetExceededOverriddenIndicatorMetadataTextValues();

            var isOverridenMetadata = multiplyOverriddenIndicatorProfilePairs.Any();

            // Write duplicated indicator metadata
            if (isOverridenMetadata)
            {
                foreach (Object[] pairs in multiplyOverriddenIndicatorProfilePairs)
                {
                    Console.WriteLine("Indicator ID" + pairs[0].ToString());
                }
            }
            Assert.IsFalse(isOverridenMetadata, "Is duplicated metadata");
        }

        [TestMethod]
        public void TestAllIndicatorMetadataMandatoryPropertiesAreDefined()
        {
            // Check mandatory metadata properties are defined for all indicators
            var indicatorIds = ReaderFactory.GetGroupDataReader().GetAllIndicatorIds();
            var provider = IndicatorMetadataProvider.Instance;
            var checker = new IndicatorMetadataChecker();
            foreach (var indicatorId in indicatorIds)
            {
                var indicatorMetadata = provider.GetIndicatorMetadata(indicatorId);
                checker.CheckMandatoryProperties(indicatorMetadata);
            }

            // Assert: no violations
            var fails = checker.Violations;
            foreach (var fail in fails)
            {
                Console.WriteLine(fail);
            }
            Assert.IsFalse(fails.Any(), "Not all mandatory metadata properties are defined");
        }

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
            var childAreaTypeIdsWithParents = GetChildAreaTypeIdsThatHaveParentOptionDefined(
                ProfileIds.Undefined);

            var areaTypeIdsOnGroupings = ReaderFactory.GetGroupDataReader()
                .GetDistinctGroupingAreaTypeIdsForAllProfiles();

            var areaTypeIdsWithoutAnyParents = new List<int>();
            foreach (var areaTypeId in areaTypeIdsOnGroupings)
            {
                if (childAreaTypeIdsWithParents.Contains(areaTypeId) == false)
                {
                    areaTypeIdsWithoutAnyParents.Add(areaTypeId);
                }
            }

            // Assert
            if (areaTypeIdsWithoutAnyParents.Any())
            {
                Assert.Fail(
                    "Default parent area type option not defined for child area type Id(s): " +
                    string.Join(",", areaTypeIdsWithoutAnyParents) + Environment.NewLine +
                    "You need to add row(s) to WS_ProfileParentAreaOptions");
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestGroupingsAreAvailableForDefaultAreaType()
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();

            var profiles = ReaderFactory.GetProfileReader().GetAllProfiles();
            foreach (var profileConfig in profiles)
            {
                var areaTypeId = profileConfig.DefaultAreaTypeId;

                // FPM test TestUpdateProfile change area type Id so need to ignore these profiles (they have ID 0 or 1)
                if (profileConfig.HasAnyData && profileConfig.ProfileId != ProfileIds.Search && 
                    areaTypeId != 0 && areaTypeId != 1)
                {
                    var groupIds = groupDataReader.GetGroupIdsOfProfile(profileConfig.ProfileId);
                    var areaTypeIds = groupDataReader.GetDistinctGroupingAreaTypeIds(groupIds);
                    if (areaTypeIds.Any() && areaTypeIds.Contains(areaTypeId) == false)
                    {
                        Assert.Fail(
                            string.Format("There are no groupings for default area type {0} for profile {1}", areaTypeId, profileConfig.ProfileId));
                    }
                }
            }
        }

        [TestMethod]
        public void TestGpPracticesAllHaveCoordinates()
        {
            var areaCodes = areasReader.GetAreaCodesForAreaType(AreaTypeIds.GpPractice);

            var splitter = new LongListSplitter<string>(areaCodes);
            while (splitter.AnyLeft())
            {
                var areas = areasReader.GetAreaWithAddressFromCodes(splitter.NextItems())
                    .Where(x => x.EastingNorthing == null);
                Assert.AreEqual(0, areas.Count(), @"Some GP practices do not have coordinates. You need to add them with Google Maps! Run query to find:
SELECT * FROM [L_Areas]
WHERE areatypeid = 7 AND areacode not in (select AreaCode from [dbo].[GIS_AreaCoordinates])");
            }
        }

        private List<int> GetChildAreaTypeIdsThatHaveParentOptionDefined(int profileId)
        {
            var defaultChildAreaTypeIds = areasReader.GetParentAreaGroupsForProfile(profileId)
                .Select(x => x.ChildAreaTypeId)
                .Distinct()
                .ToList();
            return defaultChildAreaTypeIds;
        }
    }

    public class IndicatorMetadataChecker
    {
        public IList<string> Violations { get; set; }

        public IndicatorMetadataChecker()
        {
            Violations = new List<string>();
        }

        /// <summary>
        /// Check mandatory properties are defined.
        /// </summary>
        /// <param name="indicatorMetadata"></param>
        public void CheckMandatoryProperties(IndicatorMetadata indicatorMetadata)
        {
            CheckIsDefined(indicatorMetadata, IndicatorMetadataTextColumnNames.Name);
            CheckIsDefined(indicatorMetadata, IndicatorMetadataTextColumnNames.NameLong);
            CheckIsDefined(indicatorMetadata, IndicatorMetadataTextColumnNames.DataSource);
            CheckIsDefined(indicatorMetadata, IndicatorMetadataTextColumnNames.Definition);
        }

        private void CheckIsDefined(IndicatorMetadata indicatorMetadata, string columnName)
        {
            var descriptive = indicatorMetadata.Descriptive;
            if (descriptive.ContainsKey(columnName) == false || descriptive[columnName] == null)
            {
                Violations.Add(columnName + " not defined for " + indicatorMetadata.IndicatorId);
            }
        }
    }
}
