using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.DomainObjects;
using Profiles.MainUI.Skins;
using System.Collections.Generic;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class ProfileReaderTest
    {
        private const string ProfileUrlKey = ProfileUrlKeys.Phof;
        private const string TestHostPhofUrl = "testphof.phe.org.uk";
        private const string LiveHostPhofUrl = "www.phoutcomes.info";
        private const string TestHostLongerLivesUrl = "testhealthierlives.phe.org.uk";
        private const string LiveHostLongerLivesUrl = "healthierlives.phe.org.uk";
        private const string TestEnvironment = "Test";
        private const string LiveEnvironment = "Live";

        [TestMethod]
        public void TestGetProfileCollectionItems_OrderedBySequence()
        {
            var reader = ReaderFactory.GetProfileReader();
            var items = reader.GetProfileCollectionItems(ProfileCollectionIds.NationalProfiles);
            Assert.IsTrue(items.Count > 0);
        }

        [TestMethod]
        public void TestGetProfileDetailsUrlKey()
        {
            Assert.AreEqual(ProfileUrlKey, PhofProfile().ProfileUrlKey);
        }

        [TestMethod]
        public void TestGetProfileDetailsTitle()
        {
            AssertContains("outcomes", PhofProfile().Title.ToLower());
        }

        [TestMethod]
        public void TestGetProfileDetailsDefaultAreaType()
        {
            Assert.AreEqual(102, PhofProfile().DefaultAreaType);
        }

        [TestMethod]
        public void TestGetProfileDetailsArePdfs()
        {
            Assert.IsTrue(PhofProfile().ArePdfs);
            Assert.IsFalse(SpecificProfile(ProfileUrlKeys.LongerLives).ArePdfs);
        }

        [TestMethod]
        public void TestGetProfileDetailsExtraJavaScriptFilesString()
        {
            AssertContains("+map", PhofProfile().ExtraJavaScriptFilesString);
        }

        [TestMethod]
        public void TestGetProfileDetailsExtraCssFilesString()
        {
            AssertContains("+map", PhofProfile().ExtraCssFilesString);
        }

        [TestMethod]
        public void TestGetProfileDetailsAreasToIgnoreForSpineCharts()
        {
            AssertContains("E0", PhofProfile().AreasToIgnoreForSpineCharts);
        }

        [TestMethod]
        public void TestGetProfileDetailsEnumParentDisplay()
        {
            Assert.AreEqual(0, PhofProfile().EnumParentDisplay);
        }

        [TestMethod]
        public void TestGetProfileDetailsRagColourId()
        {
            Assert.AreEqual(0, PhofProfile().RagColourId);
        }

        [TestMethod]
        public void TestGetProfileDomains()
        {
            var domains = GetDomains(new[] { GroupIds.SevereMentalIllness_Finance });
            Assert.AreEqual(1, domains.Count);
        }

        [TestMethod]
        public void TestGetProfileDomainsSubHeading()
        {
            var domains = GetDomains(new[] { 1000001 });
            AssertContains("longer", domains[0].SubHeading.ToLower());
        }

        [TestMethod]
        public void TestGetProfileDomainsReturnedInAscendingOrderOfSequence()
        {
            var firstDomain = GroupIds.SevereMentalIllness_PsychosisPathway;
            var secondDomain = GroupIds.SevereMentalIllness_RiskFactors;

            AssertFirstBeforeSecond(new[] { firstDomain, secondDomain });
            AssertFirstBeforeSecond(new[] { secondDomain, firstDomain });
        }

        private static void AssertFirstBeforeSecond(IEnumerable<int> list)
        {
            var domains = GetDomains(list);
            Assert.AreEqual(1, domains[0].Number);
            Assert.AreEqual(2, domains[1].Number);
        }

        [TestMethod]
        public void TestGetSkinFromName()
        {
            var skin = ReaderFactory.GetProfileReader().GetSkinFromName(SkinNames.Phof);
            AssertPhofSkinProperties(skin);
        }

        [TestMethod]
        public void TestGetSkinFromId()
        {
            var skin = ReaderFactory.GetProfileReader().GetSkinFromId(SkinIds.Phof);
            AssertPhofSkinProperties(skin);
        }

        [TestMethod]
        public void TestGetSkinFromTestHost()
        {
            var skin = GetSkin(TestHostPhofUrl, TestEnvironment);
            AssertPhofSkinProperties(skin);
        }

        [TestMethod]
        public void TestGetSkinFromLiveHost()
        {
            var skin = GetSkin(LiveHostPhofUrl, LiveEnvironment);
            AssertPhofSkinProperties(skin);
        }

        private static void AssertPhofSkinProperties(Skin skin)
        {
            Assert.AreEqual(SkinNames.Phof, skin.Name);
            Assert.AreEqual("Phof", skin.PartialViewFolder);
            AssertContains("outcomes", skin.TemplateProfileUrlKey);
            AssertContains("outcomes", skin.MetaDescription);
            AssertValidGoogleAnalyticsCode(skin.GoogleAnalyticsKey);
        }

        [TestMethod]
        public void TestGetTestLongerLivesSkin()
        {
            var skin = GetSkin(TestHostLongerLivesUrl, TestEnvironment);
            Assert.AreEqual(SkinNames.Mortality, skin.Name);
            Assert.IsNull(skin.PartialViewFolder);
            Assert.IsNull(skin.TemplateProfileUrlKey);
            Assert.IsNull(skin.MetaDescription);
            AssertValidGoogleAnalyticsCode(skin.GoogleAnalyticsKey);
        }

        [TestMethod]
        public void TestGetLiveLongerLivesSkin()
        {
            var skin = GetSkin(LiveHostLongerLivesUrl, LiveEnvironment);

            Assert.AreEqual(SkinNames.Mortality, skin.Name);
            Assert.IsNull(skin.PartialViewFolder);
            Assert.IsNull(skin.TemplateProfileUrlKey);
            Assert.IsNull(skin.MetaDescription);
            AssertValidGoogleAnalyticsCode(skin.GoogleAnalyticsKey);
        }

        private static Skin GetSkin(string skinName, string environment)
        {
            var profileReader = ReaderFactory.GetProfileReader();
            var skin = profileReader.GetSkin(environment, skinName);
            return skin;
        }

        [TestMethod]
        public void TestGetSkinTitle()
        {
            var skin = GetSkin(LiveHostPhofUrl, LiveEnvironment);
            AssertContains("Outcomes", skin.Title);
        }

        [TestMethod]
        public void TestGetContentItem()
        {
            var profileReader = ReaderFactory.GetProfileReader();
            var contentItem = profileReader.GetContentItem("introduction", ProfileIds.Tobacco);
            AssertContains("tobacco", contentItem.Content.ToLower());
        }

        [TestMethod]
        public void TestGetContentItemForNonExistantKey()
        {
            var profileReader = ReaderFactory.GetProfileReader();
            var contentItem = profileReader.GetContentItem("not-a-real-key", ProfileIds.Tobacco);
            Assert.IsNull(contentItem);
        }

        private static void AssertValidGoogleAnalyticsCode(string code)
        {
            AssertContains("UA-", code);
        }

        private static IList<Domain> GetDomains(IEnumerable<int> list)
        {
            var profileReader = ReaderFactory.GetProfileReader();
            return profileReader.GetProfileDomains(list);
        }

        [TestMethod]
        public void TestGetDomainIds()
        {
            var profileReader = ReaderFactory.GetProfileReader();
            var domainIds = profileReader.GetDomainIds(ProfileIds.Diabetes);
            Assert.IsTrue(domainIds.Count > 0);
        }

        [TestMethod]
        public void TestGetProfileDomainsEmptyListForNonExistantId()
        {
            var domains = GetDomains(new[] { GroupIds.DomainThatDoesNotExist });
            Assert.AreEqual(0, domains.Count);
        }

        [TestMethod]
        public void TestGetProfileDetailsById()
        {
            var details = ReaderFactory.GetProfileReader().GetProfileDetails(ProfileIds.Phof);
            Assert.AreEqual(ProfileIds.Phof, details.Id);
        }

        [TestMethod]
        public void TestGetProfileDetailsWithLongerLivesProfileDetails()
        {
            var reader = Reader();
            Assert.IsNull(reader.GetProfileDetails(ProfileIds.Phof).LongerLivesProfileDetails);

            var details = reader.GetProfileDetails(ProfileIds.Diabetes).LongerLivesProfileDetails;

            Assert.IsNotNull(details);
            Assert.IsNotNull(details.Title);
        }

        private static ProfileReader Reader()
        {
            var reader = ReaderFactory.GetProfileReader();
            return reader;
        }

        private static ProfileDetails PhofProfile()
        {
            return SpecificProfile(ProfileUrlKey);
        }

        private static ProfileDetails SpecificProfile(string urlKey)
        {
            var profileReader = ReaderFactory.GetProfileReader();
            var profileDetails = profileReader.GetProfileDetails(urlKey);
            return profileDetails;
        }

        private static void AssertContains(string segment, string full)
        {
            Assert.IsTrue(full.Contains(segment),
                string.Format("'{0}' does not contain '{1}'", full, segment));
        }

    }
}
