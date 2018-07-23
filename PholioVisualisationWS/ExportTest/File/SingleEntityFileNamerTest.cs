using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.File;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest.File
{
    [TestClass]
    public class SingleEntityFileNamerTest
    {
        private const string FileNameWithNonAlphaNumericCharacter = "a!£$%^&**()_'b";

        [TestMethod]
        public void TestDataFileName()
        {
            Assert.AreEqual("ab.data.csv", new SingleEntityFileNamer("a b").DataFileName);
        }

        [TestMethod]
        public void TestDataFileName_All_Non_AlphaNumerics_Removed()
        {
            Assert.AreEqual("ab.data.csv",
                new SingleEntityFileNamer(FileNameWithNonAlphaNumericCharacter).DataFileName);
        }

        [TestMethod]
        public void TestMetadataFileName()
        {
            Assert.AreEqual("ab.metadata.csv", new SingleEntityFileNamer("a b").MetadataFileName);
        }

        [TestMethod]
        public void TestAddressesFileName()
        {
            Assert.AreEqual("ab.addresses.csv", new SingleEntityFileNamer("a b").AddressesFileName);
        }
  
        [TestMethod]
        public void TestMetadataFileName_All_Non_AlphaNumerics_Removed()
        {
            Assert.AreEqual("ab.metadata.csv",
                new SingleEntityFileNamer(FileNameWithNonAlphaNumericCharacter).MetadataFileName);
        }

        [TestMethod]
        public void TestGetProfileMetadataFileNameForUser()
        {
            Assert.AreEqual("LocalAlcoholProfilesforEngland.metadata.csv",
                SingleEntityFileNamer.GetProfileMetadataFileNameForUser(ProfileIds.LocalAlcoholProfilesForEngland));
        }

        [TestMethod]
        public void TestGetDataForUserbyProfileAndAreaType()
        {
            Assert.AreEqual("LocalTobaccoControlProfiles-STP.data.csv",
                SingleEntityFileNamer.GetDataForUserbyProfileAndAreaType(ProfileIds.Tobacco, AreaTypeIds.Stp));
        }

        [TestMethod]
        public void TestGetAllMetadataFileNameForUser()
        {
            Assert.AreEqual("all-indicators.metadata.csv",
                SingleEntityFileNamer.GetAllMetadataFileNameForUser());
        }

        [TestMethod]
        public void TestGetDataForUserbyIndicatorAndAreaType()
        {
            Assert.AreEqual("indicators-STP.data.csv",
                SingleEntityFileNamer.GetDataForUserbyIndicatorAndAreaType(AreaTypeIds.Stp));
        }

        [TestMethod]
        public void TestDataFileNameCannotExceed100Chars()
        {
            var name = new string('x', 100);
            var filename = new SingleEntityFileNamer(name).DataFileName;

            Assert.IsTrue(filename.EndsWith(".data.csv"));
            Assert.IsTrue(filename.Length < 100);
        }

        [TestMethod]
        public void TestMetaDataFileNameCannotExceed100Chars()
        {
            var name = new string('x', 100);
            var filename = new SingleEntityFileNamer(name).MetadataFileName;

            Assert.IsTrue(filename.EndsWith(".metadata.csv"));
            Assert.IsTrue(filename.Length < 100);
        }
    }
}
