using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.LookUps;
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
            var id = AreaTypeIds.CcgsPreApr2017;
            Assert.AreEqual(id, repo.GetAreaType(id).Id);
        }

        [TestMethod]
        public void TestGetAreaTypeWithComponents()
        {
            var id = AreaTypeIds.CountyAndUnitaryAuthority;
            Assert.AreEqual(2, repo.GetAreaType(id).ComponentAreaTypes.Count);
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

        [TestMethod]
        public void TestSaveAreaType_Replace_Components()
        {
            var id = AreaTypeIds.CountyQuintile;
            var areaType = repo.GetAreaType(id);

            // Act: Save components
            SetComponentAreaTypesString(areaType, "1,2");

            // Act: Replace components
            areaType = repo.GetAreaType(id);
            SetComponentAreaTypesString(areaType, "2,3");

            // Assert: Component as expected
            var components = repo.GetAreaType(id).ComponentAreaTypes;
            Assert.AreEqual(2, components.ElementAt(0).ComponentAreaTypeId);
            Assert.AreEqual(3, components.ElementAt(1).ComponentAreaTypeId);
        }

        [TestMethod]
        public void TestSaveAreaType_Remove_Components()
        {
            var id = AreaTypeIds.CountyQuintile;
            var areaType = repo.GetAreaType(id);

            // Act: Save components
            SetComponentAreaTypesString(areaType, "1,2");

            // Act: Remove components
            areaType = repo.GetAreaType(id);
            SetComponentAreaTypesString(areaType, "");

            // Assert: Component as expected
            Assert.AreEqual(0, repo.GetAreaType(id).ComponentAreaTypes.Count);

        }

        private void SetComponentAreaTypesString(AreaType areaType, string areaTypes)
        {
            areaType.ComponentAreaTypesString = areaTypes;
            areaType.ParseComponentAreaTypesString();
            repo.SaveAreaType(areaType);
        }
    }
}

