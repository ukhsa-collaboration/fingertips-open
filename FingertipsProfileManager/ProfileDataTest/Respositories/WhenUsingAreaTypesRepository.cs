using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest
{
    [TestClass]
    public class WhenUsingAreaTypesRepository
    {
        private AreaTypeRepository repo;

        [TestInitialize]
        public void TestInitialize()
        {
            repo = new AreaTypeRepository();
        }

        [TestMethod]
        public void TestGetAreaType()
        {
            var id = AreaTypeIds.Ccg;
            Assert.AreEqual(id, repo.GetAreaType(id).Id);
        }

        [TestMethod]
        public void TestGetAllAreaTypes()
        {
            Assert.IsTrue(repo.GetAllAreaTypes().Any());
        }

        [TestMethod]
        public void TestSaveAreaType()
        {
            var id = AreaTypeIds.CountyQuintile;
            var areaType = repo.GetAreaType(id);

            // Act: change name
            var newName = Guid.NewGuid().ToString();
            areaType.Name = newName;
            repo.SaveAreaType(areaType);

            // Assert: name change in DB
            Assert.AreEqual(newName, repo.GetAreaType(id).Name);
        }

    }
}

