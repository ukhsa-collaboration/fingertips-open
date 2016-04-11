using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest
{
    [TestClass]
    public class ProfilesWriterTest
    {
        private const int GroupId = GroupIds.TobaccoControlProfiles_KeyIndicators;
        public const int TargetId = 1;
        private const string name1 = "Test1";
        private const string name2 = "Test2";
        private const int Sequence1 = 1;
        private const int Sequence2 = 2;

        [TestMethod]
        public void TestUpdate_TargetConfig()
        {
            var writer = ReaderFactory.GetProfilesWriter();

            // Set 1
            var target = writer.GetTargetById(TargetId);
            target.Description = name1;
            target.LowerLimit = 1;
            target.UpperLimit = 11;
            target.PolarityId = PolarityIds.RagLowIsGood;
            writer.UpdateTargetConfig(target);

            // Check 1
            target = writer.GetTargetById(TargetId);
            Assert.AreEqual(target.Description, name1);
            Assert.AreEqual(1, target.LowerLimit);
            Assert.AreEqual(11, target.UpperLimit);
            Assert.AreEqual(PolarityIds.RagLowIsGood, target.PolarityId);

            // Set 2
            target.Description = name2;
            target.LowerLimit = 2;
            target.UpperLimit = 12;
            target.PolarityId = PolarityIds.RagHighIsGood;
            writer.UpdateTargetConfig(target);

            // Check 2
            target = writer.GetTargetById(TargetId);
            Assert.AreEqual(target.Description, name2);
            Assert.AreEqual(2, target.LowerLimit);
            Assert.AreEqual(12, target.UpperLimit);
            Assert.AreEqual(PolarityIds.RagHighIsGood, target.PolarityId);
        }

        [TestMethod]
        public void TestUpdate_GroupingMetadataName()
        {
            var writer = ReaderFactory.GetProfilesWriter();

            // Set 1
            var metadata = writer.GetGroupingMetadata(GroupId);
            metadata.GroupName = name1;
            writer.UpdateGroupingMetadata(metadata);

            // Check 1
            metadata = writer.GetGroupingMetadata(GroupId);
            Assert.AreEqual(metadata.GroupName, name1);

            // Set 2
            metadata.GroupName = name2;
            writer.UpdateGroupingMetadata(metadata);

            // Check 2
            metadata = writer.GetGroupingMetadata(GroupId);
            Assert.AreEqual(metadata.GroupName, name2);
        }

        [TestMethod]
        public void TestReorderDomainSequence()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            var profileDetails = writer.GetProfileDetailsByProfileId(ProfileIds.Phof);
            var groupIds = ReaderFactory.GetProfilesReader().GetGroupingIds(profileDetails.Id);
            writer.ReorderDomainSequence(groupIds);

            var groupingMetadataList = writer.GetGroupingMetadataList(groupIds);
            var index = 1;
            foreach (var groupingMetadata in groupingMetadataList)
            {
                Assert.AreEqual(index++, groupingMetadata.Sequence, "Groupings not in correct order");
            }
        }

        [TestMethod]
        public void TestUpdate_GroupingMetadataNameWithApostrophe()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            string name = name1 + "'";

            // Set
            var metadata = writer.GetGroupingMetadata(GroupId);
            metadata.GroupName = name;
            writer.UpdateGroupingMetadata(metadata);

            // Check
            metadata = writer.GetGroupingMetadata(GroupId);
            Assert.AreEqual(metadata.GroupName, name);
        }

        [TestMethod]
        public void TestCreateAndDeleteContentItem()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            string contentKey = name1 + Guid.NewGuid();
            var profileId = ProfileIds.HealthProfiles;

            // Ensure no existing content item
            writer.DeleteContentItem(contentKey, profileId);
            Assert.IsNull(writer.GetContentItem(contentKey, profileId));

            // Create new item
            var contentItem = writer.NewContentItem(profileId, contentKey, "description", false, "content");
            Assert.IsTrue(contentItem.Id > 0);
            Assert.AreEqual("description", contentItem.Description);
            Assert.AreEqual("content", contentItem.Content);
            Assert.AreEqual(contentKey, contentItem.ContentKey);
            Assert.AreEqual(profileId, contentItem.ProfileId);

            // Check deletion
            writer.DeleteContentItem(contentKey, profileId);
            Assert.IsNull(writer.GetContentItem(contentKey, profileId));
        }

        [TestMethod]
        public void TestCreateAndDeleteTargetConfig()
        {
            var writer = ReaderFactory.GetProfilesWriter();

            var target = writer.NewTargetConfig(new TargetConfig
            {
                Description = "Test1",
                LowerLimit = 3,
                UpperLimit = 4,
                PolarityId = PolarityIds.RagLowIsGood
            });

            // Create new item
            Assert.IsTrue(target.Id > 0);
            Assert.AreEqual(target.Description, "Test1");
            Assert.AreEqual(3, target.LowerLimit);
            Assert.AreEqual(4, target.UpperLimit);
            Assert.AreEqual(PolarityIds.RagLowIsGood, target.PolarityId);

            // Check deletion
            writer.DeleteTargetConfig(target);
            Assert.IsNull(writer.GetTargetById(target.Id));
        }

        [TestMethod]
        public void TestUpdateContentItem()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            string contentKey = name1 + Guid.NewGuid();
            var profileId = ProfileIds.HealthProfiles;

            // Create new item
            var contentItem = writer.NewContentItem(profileId, contentKey, "a", false, "b");
            Assert.AreEqual(contentItem.Description, "a");
            Assert.AreEqual(contentItem.Content, "b");

            // Change properties
            contentItem.Description = "c";
            contentItem.Content = "d";

            // Update and verify
            writer.UpdateContentItem(contentItem);
            var contentItem2 = writer.GetContentItem(contentItem.Id);
            Assert.AreEqual(contentItem2.Description, "c");
            Assert.AreEqual(contentItem2.Content, "d");
        }

        [TestMethod]
        public void TestDeleteContentItemWithNonExistantKeyFailsSilently()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            string contentKey = name1 + Guid.NewGuid();
            writer.DeleteContentItem(contentKey, ProfileIds.HealthProfiles);
        }

        [TestMethod]
        public void TestUpdate_GroupingMetadataSequence()
        {
            var writer = ReaderFactory.GetProfilesWriter();

            // Set 1
            var metadata = writer.GetGroupingMetadata(GroupId);
            metadata.Sequence = Sequence1;
            writer.UpdateGroupingMetadata(metadata);

            // Check 1
            metadata = writer.GetGroupingMetadata(GroupId);
            Assert.AreEqual(metadata.Sequence, Sequence1);

            // Set 2
            metadata.Sequence = Sequence2;
            writer.UpdateGroupingMetadata(metadata);

            // Check 2
            metadata = writer.GetGroupingMetadata(GroupId);
            Assert.AreEqual(metadata.Sequence, Sequence2);
        }

        [TestMethod]
        public void TestUpdate_Grouping()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            var groupId = GroupIds.PhofWiderDeterminantsOfHealth;

            // Set 1
            var grouping = writer.GetGroupings(groupId).First();
            var yearRange = grouping.YearRange;
            grouping.YearRange = 20;
            writer.UpdateGroupingList(new List<Grouping> { grouping });

            // Check 1
            grouping = writer.GetGroupings(groupId).First();
            Assert.AreEqual(grouping.YearRange, 20);

            // Set 2
            grouping.YearRange = yearRange;
            writer.UpdateGroupingList(new List<Grouping> { grouping });

            // Check 2
            grouping = writer.GetGroupings(groupId).First();
            Assert.AreEqual(yearRange, grouping.YearRange);
        }

        [TestMethod]
        public void TestSaveNewThenDeleteGroupingMetadata()
        {
            var writer = ReaderFactory.GetProfilesWriter();

            // Save new metadata
            var metadata = writer.NewGroupingMetadata(name1, Sequence1, ProfileIds.Diabetes);
            var groupId = metadata.GroupId;

            // Check metadata was persisted
            var metadataFromDatabase = writer.GetGroupingMetadata(groupId);
            Assert.AreNotEqual(0, metadataFromDatabase.GroupId);
            Assert.AreEqual(name1, metadataFromDatabase.GroupName);
            Assert.AreEqual(name1, metadataFromDatabase.Description);
            Assert.AreEqual(Sequence1, metadataFromDatabase.Sequence);

            // Delete metadata
            writer.DeleteGroupingMetadata(groupId);

            // Check metadata was deleted
            Assert.IsNull(writer.GetGroupingMetadata(groupId));
        }

        [TestMethod]
        public void TestNewContentAudit()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            string contentKey = name1 + Guid.NewGuid();
            var profileId = ProfileIds.HealthProfiles;

            // Create new item to get valid content ID
            var contentItem = writer.NewContentItem(profileId, contentKey, "description", false, "content");

            var contentaudit = new ContentAudit
            {
                ContentId = contentItem.Id,
                ContentKey = contentKey,
                FromContent = "from",
                ToContent = "to",
                Timestamp = DateTime.Now,
                UserName = name1
            };

            var contentAudit = writer.NewContentAudit(contentaudit);
            Assert.IsTrue(contentAudit.Id > 0);
        }

        [TestMethod]
        public void TestNewDocument()
        {
            var writer = ReaderFactory.GetProfilesWriter();
            var doc = new Document
            {
                ProfileId = ProfileIds.Diabetes,
                FileName = Guid.NewGuid().ToString(),
                FileData = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 },
                UploadedBy = "Test User",
                UploadedOn = DateTime.Now
            };

            var id = writer.NewDocument(doc);
            Assert.IsTrue(id > 0);
            
        }

        [TestMethod]
        public void TestUploadDocument()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var writer = ReaderFactory.GetProfilesWriter();

            // get copy from database
            var docFromDb = reader.GetDocuments(ProfileIds.Diabetes).First();
           
            // clone and update fields 
            var docToUpdate = new Document
            {
                Id = docFromDb.Id,
                ProfileId = docFromDb.ProfileId,
                FileName =  docFromDb.FileName,
                FileData = new byte[] { 0x5, 0x6, 0x7, 0x8, 0x9 },
                UploadedBy = docFromDb.UploadedBy,
                UploadedOn = DateTime.Now
            };

            // update cloned copy
            writer.UpdateDocument(docToUpdate);

            // get cloned copy from datas
            var updatedDoc = reader.GetDocument(docToUpdate.Id);

            Assert.AreNotEqual(updatedDoc.FileData, docFromDb.FileData);
            Assert.AreNotEqual(updatedDoc.UploadedOn, docFromDb.UploadedOn);
        }

        [TestMethod]
        public void TestCopyIndicatorMetadataTextValue()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var writer = ReaderFactory.GetProfilesWriter();
            const int targetProfile = ProfileIds.Diabetes;
            const int indicatorId = IndicatorIds.ChildrenInPoverty;

            // get an indicator metadata text values from database
            var indicatorTextValues = reader.GetMetadataTextValueForAnIndicatorById(
                indicatorId, ProfileIds.HealthProfiles);

            // make a copy
            var copy = new IndicatorMetadataTextValue
            {
                IndicatorId = indicatorId,
                ProfileId = targetProfile,
                Name = indicatorTextValues.Name,
                NameLong = indicatorTextValues.NameLong,
                Definition = indicatorTextValues.Definition,
                Rationale = indicatorTextValues.Rationale,
                Policy = indicatorTextValues.Policy,
                DataSource = indicatorTextValues.DataSource,
                Producer = indicatorTextValues.Producer,
                IndMethod = indicatorTextValues.IndMethod,
                StandardPop = indicatorTextValues.StandardPop,
                CIMethod = indicatorTextValues.CIMethod,
                CountSource = indicatorTextValues.CountSource,
                CountDefinition = indicatorTextValues.CountDefinition,
                DenomSource = indicatorTextValues.DenomSource,
                DenomDefinition = indicatorTextValues.DenomDefinition,
                DiscControl = indicatorTextValues.DiscControl,
                Caveats = indicatorTextValues.Caveats,
                Copyright = indicatorTextValues.Copyright,
                Reuse = indicatorTextValues.Reuse,
                Links = indicatorTextValues.Links,
                RefNum = indicatorTextValues.RefNum,
                Notes = indicatorTextValues.Notes,
                Frequency = indicatorTextValues.Frequency,
                Rounding = indicatorTextValues.Rounding,
                DataQuality = indicatorTextValues.DataQuality
            };

            // Delete existing metadata text values
            writer.DeleteOverriddenIndicatorMetadataTextValue(copy);

            // write copy to db
            writer.NewIndicatorMetadataTextValue(copy);

            // get copy from db
            var copyFromDb = reader.GetMetadataTextValueForAnIndicatorById(indicatorId, targetProfile);

            Assert.AreNotEqual(copy,copyFromDb);
            Assert.AreEqual(copy.IndicatorId, copyFromDb.IndicatorId);
            Assert.AreEqual(copy.ProfileId, copyFromDb.ProfileId);
            Assert.AreEqual(copy.NameLong, copyFromDb.NameLong);
        }

    }
}
