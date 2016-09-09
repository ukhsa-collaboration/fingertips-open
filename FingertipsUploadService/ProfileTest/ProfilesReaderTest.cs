using FingertipsUploadService.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FingertipsUploadService.ProfileDataTest
{
    [TestClass]
    public class ProfilesReaderTest
    {
        [TestMethod]
        public void TestGetCategories()
        {
            Assert.IsNotNull(Reader().GetAllCategories());
        }

        [TestMethod]
        public void TestGetAllAgeIds()
        {
            Assert.IsTrue(Reader().GetAllAgeIds().Any());
        }

        [TestMethod]
        public void TestGetAllValueNoteIds()
        {
            Assert.IsTrue(Reader().GetAllValueNoteIds().Any());
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
        public void TestGetOwnerProfilesByIndicatorIds()
        {
            var profile = Reader().GetOwnerProfilesByIndicatorIds(
                IndicatorIds.ChildrenInPoverty);
            Assert.IsNotNull(profile);
        }

        [TestMethod]
        public void TestGetOwnerProfilesByIndicatorIds_ForIndicatorThatDoesNotExist()
        {
            var profile = Reader().GetOwnerProfilesByIndicatorIds(
                IndicatorIds.IndicatorThatDoesNotExist);
            Assert.IsNull(profile);
        }

        private static ProfilesReader Reader()
        {
            return ReaderFactory.GetProfilesReader();
        }
    }
}