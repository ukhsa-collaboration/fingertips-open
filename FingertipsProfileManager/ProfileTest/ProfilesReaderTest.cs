using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProfileDataTest
{
    [TestClass]
    public class ProfilesReaderTest
    {
        [TestMethod]
        public void TestGetCoreDataSetTimePeriods()
        {
            var grouping = new GroupingPlusName
            {
                YearRange = 1,
                IndicatorId = IndicatorIds.ObesityYear6,
                AgeId = AgeIds.Years10To11,
                SexId = SexIds.Persons,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            };
            IList<TimePeriod> periods = Reader().GetCoreDataSetTimePeriods(grouping);
            Assert.IsTrue(periods.Any());
            Assert.IsTrue(periods.Count < 20, "distinct might not be included in query if too many results");
            Assert.IsTrue(periods.First().Year < periods.Last().Year);
        }

        [TestMethod]
        public void TestSearchIndicatorTest()
        {
            IList<IndicatorMetadataTextValue> indicatorTextValues =
                Reader().SearchIndicatorMetadataTextValuesByText("%obesity%");
            Assert.IsTrue(indicatorTextValues.Count > 0);
        }

        [TestMethod]
        public void TestGetMetadataTextValueForAnIndicatorById()
        {
            var indicatorTextValues = Reader().GetMetadataTextValueForAnIndicatorById(IndicatorIds.ChildrenInPoverty, ProfileIds.HealthProfiles);
            Assert.IsNotNull(indicatorTextValues);
        }

        [TestMethod]
        public void TestGetAllAreaTypes()
        {
            Assert.IsTrue(Reader().GetAllAreaTypes().Any());
        }

        [TestMethod]
        public void TestGetTargetById()
        {
            Assert.IsNotNull(Reader().GetTargetById(1));
        }

        [TestMethod]
        public void TestGetCategories()
        {
            Assert.IsNotNull(Reader().GetAllCategories());
        }

        [TestMethod]
        public void TestGetAllTargets()
        {
            Assert.IsNotNull(Reader().GetAllTargets());
        }

        [TestMethod]
        public void TestGetCategoriesByCategoryTypeId()
        {
            Assert.AreEqual(7,
                Reader().GetCategoriesByCategoryTypeId(CategoryTypeIds.EthnicGroups7Categories).Count);
        }

        [TestMethod]
        public void TestGetSupportedAreaTypes()
        {
            ProfilesReader reader = Reader();
            IList<AreaType> areaTypes = reader.GetSupportedAreaTypes();
            Assert.IsTrue(areaTypes.Any());
            Assert.IsTrue(areaTypes.Count < reader.GetAllAreaTypes().Count);
        }

        [TestMethod]
        public void TestGetAreaTypes()
        {
            IList<AreaType> types = Reader().GetSpecificAreaTypes(new List<int> {2});
            Assert.IsTrue(types.First().ShortName.Contains("Primary"));
        }

        [TestMethod]
        public void TestGetAllAgeIds()
        {
            Assert.IsTrue(Reader().GetAllAgeIds().Any());
        }

        [TestMethod]
        public void TestGetAllAges()
        {
            Assert.IsTrue(Reader().GetAllAges().Any());
        }

        [TestMethod]
        public void TestGetAllValueNoteIds()
        {
            Assert.IsTrue(Reader().GetAllValueNoteIds().Any());
        }

        [TestMethod]
        public void TestGetAllValueNotes()
        {
            Assert.IsTrue(Reader().GetAllValueNotes().Any());
        }

        [TestMethod]
        public void TestGetAllAreaCodes()
        {
            Assert.IsTrue(Reader().GetAllAreaCodes().Any());
        }

        [TestMethod]
        public void TestGetAllSexIds()
        {
            Assert.IsTrue(Reader().GetAllSexIds().Any());
        }

        [TestMethod]
        public void TestGetAllSexes()
        {
            Assert.IsTrue(Reader().GetAllSexes().Any());
        }

        [TestMethod]
        public void TestGetUserGroupPermissions()
        {
            IList<UserGroupPermissions> userGroupPermissions = Reader()
                .GetUserGroupPermissionsByProfileId(ProfileIds.UnassignedIndicators);
            Assert.IsTrue(userGroupPermissions.Any());

            //Ensure that the user has assigned permissions to profileId 23 ('unassigned-indicators')
            Assert.IsTrue(userGroupPermissions.Any(x => x.ProfileId == ProfileIds.UnassignedIndicators));
        }

        [TestMethod]
        public void TestGetProfiles()
        {
            Assert.IsTrue(Reader().GetProfiles().Count > 0);
        }

        [TestMethod]
        public void TestGetProfileDetails()
        {
            ProfileDetails details = Reader().GetProfileDetails(UrlKeys.Phof);
            Assert.IsNotNull(details);
            Assert.IsTrue(details.ArePdfs);
        }

        [TestMethod]
        public void TestGetGroupings()
        {
            Assert.IsTrue(Reader().GetGroupings(GroupIds.PhofWiderDeterminantsOfHealth).Count > 10);
        }

        [TestMethod]
        public void TestGetGroupingsByGroupIdAndAreaTypeId_InSequenceOrder()
        {
            IList<Grouping> groupings = Reader().GetGroupings(GroupIds.PhofWiderDeterminantsOfHealth);

            Assert.IsTrue(groupings.Any());

            int previousSequence = 0;
            foreach (Grouping grouping in groupings)
            {
                Assert.IsTrue(grouping.Sequence >= previousSequence);
                previousSequence = grouping.Sequence;
            }
        }

        [TestMethod]
        public void TestGetSpecificGroupings()
        {
            int yearRange = 1;

            IList<Grouping> groupings = Reader().GetGroupings(
                new List<int> {GroupIds.PhofWiderDeterminantsOfHealth},
                AreaTypeIds.CountyAndUnitaryAuthority,
                IndicatorIds.ChildrenInPoverty,
                SexIds.Persons,
                AgeIds.LessThan16,
                yearRange
                );

            Assert.IsTrue(groupings.Any());

            foreach (Grouping grouping in groupings)
            {
                Assert.AreEqual(GroupIds.PhofWiderDeterminantsOfHealth, grouping.GroupId);
                Assert.AreEqual(AreaTypeIds.CountyAndUnitaryAuthority, grouping.AreaTypeId);
                Assert.AreEqual(IndicatorIds.ChildrenInPoverty, grouping.IndicatorId);
                Assert.AreEqual(SexIds.Persons, grouping.SexId);
                Assert.AreEqual(AgeIds.LessThan16, grouping.AgeId);
                Assert.AreEqual(yearRange, grouping.YearRange);
            }
        }

        [TestMethod]
        public void TestGetGroupingsByGroupIdAreaTypeIdIndicatorIdAndSexId()
        {
            IList<Grouping> groupings = Reader().GetGroupingsByGroupIdAreaTypeIdIndicatorIdAndSexId(
                GroupIds.PhofWiderDeterminantsOfHealth,
                AreaTypeIds.CountyAndUnitaryAuthority,
                IndicatorIds.ChildrenInPoverty,
                SexIds.Persons
                );

            Assert.IsTrue(groupings.Any());

            foreach (Grouping grouping in groupings)
            {
                Assert.AreEqual(GroupIds.PhofWiderDeterminantsOfHealth, grouping.GroupId);
                Assert.AreEqual(AreaTypeIds.CountyAndUnitaryAuthority, grouping.AreaTypeId);
                Assert.AreEqual(IndicatorIds.ChildrenInPoverty, grouping.IndicatorId);
                Assert.AreEqual(SexIds.Persons, grouping.SexId);
            }
        }

        [TestMethod]
        public void TestGetGroupingByIndicatorId()
        {
            var indicatorIds = new List<int> { IndicatorIds.IDAOPI };

            Assert.IsTrue(Reader().GetGroupingByIndicatorId(indicatorIds).Any());
        }

        [TestMethod]
        public void TestGetGroupingMetadata()
        {
            IList<GroupingMetadata> metadata = Reader().GetGroupingMetadataList(
                new List<int>
                {
                    GroupIds.SevereMentalIllness_Prevalence,
                    GroupIds.SevereMentalIllness_Finance
                });

            Assert.AreEqual(2, metadata.Count);

            Assert.IsTrue(metadata[0].GroupName.ToLower().Contains("prevalence"));
            Assert.IsTrue(metadata[1].GroupName.ToLower().Contains("finance"));
        }

        [TestMethod]
        public void TestGetGroupingMetadataPracticeProfiles2012()
        {
            IList<GroupingMetadata> metadata = Reader().GetGroupingMetadataList(
                new List<int> {GroupIds.GpProfileSupportingIndicators});

            Assert.AreEqual(1, metadata.Count);

            Assert.IsTrue(metadata[0].GroupName.Contains("GP"));
        }

        [TestMethod]
        public void TestGetTextProperties()
        {
            ProfilesReader reader = Reader();
            Assert.IsTrue(reader.GetIndicatorMetadataTextProperties().Count > 10);
        }

        [TestMethod]
        public void TestGetIndicatorTextValuesForNonExistantIndicator()
        {
            ProfilesReader reader = Reader();
            IList<IndicatorMetadataTextProperty> properties = reader.GetIndicatorMetadataTextProperties();
            IList<IndicatorText> list = reader.GetIndicatorTextValues(987654321/*not a real indicator id*/, properties, null);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void TestGetIndicatorTextValuesGeneric()
        {
            ProfilesReader reader = Reader();
            IList<IndicatorMetadataTextProperty> properties = reader.GetIndicatorMetadataTextProperties();
            IList<IndicatorText> list = reader.GetIndicatorTextValues(IndicatorIds.ChildrenInPoverty, properties, null);
            Assert.IsTrue(list.Count > 10);
        }

        [TestMethod]
        public void TestGetIndicatorTextValuesGroupSpecificAvailable()
        {
            ProfilesReader reader = Reader();
            IList<IndicatorMetadataTextProperty> properties = reader.GetIndicatorMetadataTextProperties();
            IList<IndicatorText> list = reader.GetIndicatorTextValues(
                IndicatorIds.LifeExpectancyAtBirth, properties, ProfileIds.ChildrenAndYoungPeoplesHealthBenchmarkingTool);
            Assert.IsFalse(list[0].ValueSpecific.Contains("0.1i"));
            Assert.IsTrue(list[0].ValueGeneric.Contains("0.1i"));
            Assert.IsTrue(list.Count > 10);
        }

        [TestMethod]
        public void TestGetIndicatorTextValuesGroupSpecificIgnoredIfNotAppropriate()
        {
            ProfilesReader reader = Reader();
            IList<IndicatorMetadataTextProperty> properties = reader.GetIndicatorMetadataTextProperties();
            IList<IndicatorText> list = reader.GetIndicatorTextValues(
                IndicatorIds.HipFractures, properties, ProfileIds.ProfileThatDoesNotExist);
            Assert.IsNull(list[0].ValueSpecific);
            Assert.IsTrue(list[0].ValueGeneric.Contains("4.14"));
            Assert.IsTrue(list.Count > 10);
        }

        [TestMethod]
        public void TestAreaTextSearchInDb()
        {
            IEnumerable<Area> areas;
            ProfilesReader reader = Reader();

            //Test for CCG 
            areas = reader.GetAreas("Cam", AreaTypeIds.Ccg);
            Assert.IsTrue(areas.Any());

            //Test for County/UA 
            areas = reader.GetAreas("Cam", AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(areas.Any());

            //Test for LA/UA 
            areas = reader.GetAreas("Cam", AreaTypeIds.LocalAuthorityAndUnitaryAuthority);
            Assert.IsTrue(areas.Any());

            //Test for CCG - Not Found 
            areas = reader.GetAreas("Camx", AreaTypeIds.Ccg);
            Assert.IsFalse(areas.Any());
        }

        [TestMethod]
        public void TestGetAreas_AllAreaTypesSearchedIfAreaTypeIdIsNull()
        {
            ProfilesReader reader = Reader();
            int onlyCcg = reader.GetAreas("Cam", AreaTypeIds.Ccg).Count();
            int allAreaTypes = reader.GetAreas("Cam", null).Count();
            Assert.IsTrue(onlyCcg < allAreaTypes);
        }

        [TestMethod]
        public void TestGetAreas_OnlyReturnsAreaOfCurrentTypes()
        {
            IEnumerable<Area> areas = Reader().GetAreas("Cam", null);

            foreach (Area area in areas)
            {
                Assert.AreNotEqual(AreaTypeIds.CountyQuintile, area.AreaTypeId);
            }
        }

        [TestMethod]
        public void TestGetAreasReturnMaxAllowedResults()
        {
            IEnumerable<Area> areas = Reader().GetAreas("%%", AreaTypeIds.GpPractice);
            Assert.AreEqual(ProfilesReader.MaxAreaResults, areas.Count());
        }

        [TestMethod]
        public void TestGetAllComparatorConfidences()
        {
            IList<double> list = Reader().GetAllComparatorConfidences();
            var expected = new List<double> {95, 98, 99.8};
            Assert.AreEqual(list.Count, expected.Count);
            foreach (double val in expected)
            {
                Assert.AreEqual(1, list.Count(x => x.Equals(val)));
            }
        }

        [TestMethod]
        public void TestGetAllPolarities()
        {
            Assert.IsTrue(Reader().GetAllPolarities().Any());
        }

        [TestMethod]
        public void TestGetAllComparatorMethods()
        {
            Assert.IsTrue(Reader().GetAllComparatorMethods().Any());
        }

        [TestMethod]
        public void TestGetDocument()
        {
            var filename = Guid.NewGuid().ToString();
            SaveNewDocument(filename);
            IList<Document> docList = GetDocumentsFromDb();
            Document doc = Reader().GetDocument(docList.ToList().First().Id);
            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public void TestGetDocuments()
        {
            IList<Document> docList = GetDocumentsFromDb();
            Assert.IsTrue(docList.Count > 0);
        }

        [TestMethod]
        public void TestGetExceptionsByServer()
        {
            var server = GetTestServerName();
            IList<ExceptionLog> exceptions1day = Reader().GetExceptionsByServer(1, server);
            IList<ExceptionLog> exceptions10days = Reader().GetExceptionsByServer(10, server);
            Assert.IsTrue(exceptions10days.Count > exceptions1day.Count);
        }

        [TestMethod]
        public void TestGetExceptionsForAllServers()
        {
            const int days = 10;
            IList<ExceptionLog> allServers = Reader().GetExceptionsForAllServers(days);
            IList<ExceptionLog> oneServer = Reader().GetExceptionsByServer(days, GetTestServerName());
            Assert.IsTrue(allServers.Count > oneServer.Count,
                "Expected more exceptions from all servers than just the test server");
        }

        [TestMethod]
        public void TestGetOwnerProfilesByIndicatorIds()
        {
            var profile = Reader().GetOwnerProfilesByIndicatorIds(
                IndicatorIds.IDAOPI);
            Assert.IsNotNull(profile);
        }

        [TestMethod]
        public void TestGetOwnerProfilesByIndicatorIds_ForIndicatorThatDoesNotExist()
        {
            var profile = Reader().GetOwnerProfilesByIndicatorIds(
                IndicatorIds.IndicatorThatDoesNotExist);
            Assert.IsNull(profile);
        }


        private static string GetTestServerName()
        {
            return ConfigurationManager.AppSettings["TestServerName"];
        }

        private void SaveNewDocument(string name)
        {
            var doc = new Document
            {
                ProfileId = ProfileIds.Diabetes,
                FileName = name,
                FileData = new byte[] {0x1, 0x2, 0x3, 0x4, 0x5},
                UploadedBy = "Doris",
                UploadedOn = DateTime.Now
            };
            Writer().NewDocument(doc);
        }

        private IList<Document> GetDocumentsFromDb()
        {
            return Reader().GetDocuments(ProfileIds.Diabetes);
        }

        private static ProfilesReader Reader()
        {
            return ReaderFactory.GetProfilesReader();
        }

        private static ProfilesWriter Writer()
        {
            return ReaderFactory.GetProfilesWriter();
        }
    }
}