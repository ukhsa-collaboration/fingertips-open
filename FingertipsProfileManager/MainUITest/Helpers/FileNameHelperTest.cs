using System;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class FileNameHelperTest
    {
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();
        Document docA = new Document
        {
            ProfileId = ProfileIds.Diabetes,
            FileName = "A.doc",
            FileData = new byte[] { 0x5, 0x6, 0x7, 0x8, 0x9 },
            UploadedBy = UserNames.Doris,
            UploadedOn = DateTime.Now
        };

        Document docB = new Document
        {
            ProfileId = ProfileIds.HealthProfiles,
            FileName = "B.doc",
            FileData = new byte[] { 0x5, 0x6, 0x7, 0x8, 0x9 },
            UploadedBy = UserNames.Doris,
            UploadedOn = DateTime.Now
        };

        [TestMethod]
        public void TestFileNameUniqueness()
        {                       
            var fileNameHelper = new FileNameHelper(ReaderFactory.GetProfilesReader());
              
            // Check for override
            var overrideUniqueness = fileNameHelper.IsUnique(docA.FileName, docA.ProfileId);
            Assert.AreEqual(FileNameHelper.Uniqueness.UniqueToProfile, overrideUniqueness);

            // Check same filname for different profile
            var notUnique = fileNameHelper.IsUnique(docA.FileName, ProfileIds.Phof);
            Assert.AreEqual(FileNameHelper.Uniqueness.NotUnique, notUnique);

            // Check a unique filename 
            var unique = fileNameHelper.IsUnique(docB.FileName, docB.ProfileId);
            Assert.AreEqual(FileNameHelper.Uniqueness.Unique, unique);
        }

        [TestInitialize]
        public void Setup()
        {
            // Add a unique document to database
            _writer.NewDocument(docA);
        }

        [TestCleanup]
        public void Clearup()
        {
            _writer.DeleteDocument(docA);
        }
    }
}
