using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.CkanTest
{
    [TestClass]
    public class CkanPackageTest
    {
        [TestMethod]
        public void TestIsInstanceFromRepository_False()
        {
            var package = new CkanPackage();
            Assert.IsFalse(package.IsInstanceFromRepository);
        }

        [TestMethod]
        public void TestIsInstanceFromRepository_True()
        {
            var package = new CkanPackage{MetadataCreated = DateTime.Now};
            Assert.IsTrue(package.IsInstanceFromRepository);
        }
    }
}
