using System;
using System.Collections.Generic;
using System.Linq;
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
            var indicatorIds = ReaderFactory.GetGroupDataReader().GetAllIndicators();
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
