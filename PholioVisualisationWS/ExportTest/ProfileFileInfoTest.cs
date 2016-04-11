
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
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
                AreaTypeIds.CountyAndUnitaryAuthority,
                AreaTypeIds.PheCentreObsolete);
            Assert.AreEqual(profileId + "-A-102-43.xlsx", info.FileName);
        }

        [TestMethod]
        public void TestFileExtension_XlsWithTobacco()
        {
            var profileId = ProfileIds.Tobacco;

            ProfileFileInfo info = new ProfileFileInfo(
               profileId,
                new List<string> { "A" },
                AreaTypeIds.CountyAndUnitaryAuthority,
                AreaTypeIds.PheCentreObsolete);
            Assert.AreEqual(profileId + "-A-102-43.xlsx", info.FileName);
        }

        [TestMethod]
        public void TestFileExtension_Xlsx()
        {
            ProfileFileInfo fileInfo = new ProfileFileInfo(
                ProfileIds.Phof,
                new List<string> { "A" },
                AreaTypeIds.Pct,
                AreaTypeIds.PheCentreObsolete);
            Assert.AreEqual("xlsx", fileInfo.FileExtension);
        }

        [TestMethod]
        public void TestFileName()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.SubstanceMisuse,
                new List<string> { "A" },
                AreaTypeIds.Pct,
                AreaTypeIds.PheCentreObsolete);
            Assert.AreEqual(ProfileIds.SubstanceMisuse + "-A-2-43.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestFileNameWithMoreThanOneAreaCode()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.SubstanceMisuse,
                new List<string> { "A", "B" },
                AreaTypeIds.Pct,
                AreaTypeIds.PheCentreObsolete);
            Assert.AreEqual(ProfileIds.SubstanceMisuse + "-A-B-2-43.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestAreaCodesSortedAlphabetically()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.SubstanceMisuse,
                new List<string> { "X", "A", "B" },
                AreaTypeIds.Pct,
                AreaTypeIds.PheCentreObsolete);
            Assert.AreEqual(ProfileIds.SubstanceMisuse + "-A-B-X-2-43.xlsx", maker.FileName);
        }

        [TestMethod]
        public void TestFilePath()
        {
            ProfileFileInfo maker = new ProfileFileInfo(
                ProfileIds.SubstanceMisuse,
                 new List<string> { "A" },
                AreaTypeIds.CountyAndUnitaryAuthority,
                AreaTypeIds.PheCentreObsolete);

            var expectedFileName = ProfileIds.SubstanceMisuse + "-A-102-43.xlsx";
            var expectedFilePath = Path.Combine(ApplicationConfiguration.ExportFileDirectory, expectedFileName);
            Assert.AreEqual(expectedFilePath, maker.FilePath);
        }

    }
}
