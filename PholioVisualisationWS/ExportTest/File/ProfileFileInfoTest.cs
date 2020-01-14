
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.File;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest.File
{
    [TestClass]
    public class ProfileFileInfoTest
    {
        [TestMethod]
        public void TestFileExtension_XlsxWithPhof()
        {
            var profileId = ProfileIds.Phof;

            ProfileFileInfo info = new ProfileFileInfo(
               profileId,
                new List<string> { "A" },
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                AreaTypeIds.GoRegion);
            Assert.AreEqual(profileId + "-A-102-6.xlsx", info.FileName);
        }

        [TestMethod]
        public void TestFileExtension_XlsWithTobacco()
        {
            var profileId = ProfileIds.Tobacco;

            ProfileFileInfo info = new ProfileFileInfo(
               profileId,
                new List<string> { "A" },
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                AreaTypeIds.GoRegion);
            Assert.AreEqual(profileId + "-A-102-6.xlsx", info.FileName);
        }

        [TestMethod]
        public void TestFileExtension_Xlsx()
        {
            ProfileFileInfo fileInfo = new ProfileFileInfo(
                ProfileIds.Phof,
                new List<string> { "A" },
                AreaTypeIds.Pct,
                AreaTypeIds.GoRegion);
            Assert.AreEqual("xlsx", fileInfo.FileExtension);
        }

        [TestMethod]
        public void TestFileName()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.HealthAssetsProfile,
                new List<string> { "A" },
                AreaTypeIds.Pct,
                AreaTypeIds.GoRegion);
            Assert.AreEqual(ProfileIds.HealthAssetsProfile + "-A-2-6.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestFileNameWithMoreThanOneAreaCode()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.HealthAssetsProfile,
                new List<string> { "A", "B" },
                AreaTypeIds.Pct,
                AreaTypeIds.GoRegion);
            Assert.AreEqual(ProfileIds.HealthAssetsProfile + "-A-B-2-6.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestAreaCodesSortedAlphabetically()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.HealthAssetsProfile,
                new List<string> { "X", "A", "B" },
                AreaTypeIds.Pct,
                AreaTypeIds.GoRegion);
            Assert.AreEqual(ProfileIds.HealthAssetsProfile + "-A-B-X-2-6.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestFilePath()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.HealthAssetsProfile,
                 new List<string> { "A" },
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                AreaTypeIds.GoRegion);

            var expectedFileName = ProfileIds.HealthAssetsProfile + "-A-102-6.xlsx";
            var expectedFilePath = Path.Combine(ApplicationConfiguration.Instance.ExportFileDirectory, expectedFileName);
            Assert.AreEqual(expectedFilePath, maker.FilePath);
        }

    }
}
