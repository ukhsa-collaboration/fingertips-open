using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.DataTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.CkanTest.DataTransformation
{
    [TestClass]
    public class CkanFileNamerTest
    {
        private const string FileNameWithNonAlphaNumericCharacter = "a-!£$%^&**()_'b";

        [TestMethod]
        public void TestDataFileName()
        {
            Assert.AreEqual("ab.data.csv", new CkanFileNamer("a b").DataFileName);
        }

        [TestMethod]
        public void TestDataFileName_All_Non_AlphaNumerics_Removed()
        {
            Assert.AreEqual("ab.data.csv", 
                new CkanFileNamer(FileNameWithNonAlphaNumericCharacter).DataFileName);
        }

        [TestMethod]
        public void TestMetadataFileName()
        {
            Assert.AreEqual("ab.metadata.csv", new CkanFileNamer("a b").MetadataFileName);
        }

        [TestMethod]
        public void TestMetadataFileName_All_Non_AlphaNumerics_Removed()
        {
            Assert.AreEqual("ab.metadata.csv", 
                new CkanFileNamer(FileNameWithNonAlphaNumericCharacter).MetadataFileName);
        }

        [TestMethod]
        public void TestDataFileNameCannotExceed100Chars()
        {
            var name = new string('x', 100);
            var filename = new CkanFileNamer(name).DataFileName;

            Assert.IsTrue(filename.EndsWith(".data.csv"));
            Assert.IsTrue(filename.Length < 100);
        }

        [TestMethod]
        public void TestMetaDataFileNameCannotExceed100Chars()
        {
            var name = new string('x', 100);
            var filename = new CkanFileNamer(name).MetadataFileName;

            Assert.IsTrue(filename.EndsWith(".metadata.csv"));
            Assert.IsTrue(filename.Length < 100);
        }
    }
}
