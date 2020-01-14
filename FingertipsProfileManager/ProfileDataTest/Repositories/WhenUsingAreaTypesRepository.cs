using Fpm.ProfileData;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingAreaTypesRepository
    {
        private AreaTypeRepository _areaTypeRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _areaTypeRepository = new AreaTypeRepository();
        }

        [TestMethod]
        public void TestGetAreaType()
        {
            var id = AreaTypeIds.CcgsPreApr2017;
            Assert.AreEqual(id, _areaTypeRepository.GetAreaType(id).Id);
        }

        [TestMethod]
        public void TestGetAreaTypeWithComponents()
        {
            var id = AreaTypeIds.CountyAndUnitaryAuthorityPre2019;
            Assert.AreEqual(4, _areaTypeRepository.GetAreaType(id).ComponentAreaTypes.Count);
        }

        [TestMethod]
        public void TestGetAllAreaTypes()
        {
            Assert.IsTrue(_areaTypeRepository.GetAllAreaTypes().Any());
        }

        [TestMethod]
        public void TestSaveAreaType()
        {
            var id = AreaTypeIds.CountyQuintile;
            var areaType = _areaTypeRepository.GetAreaType(id);

            // Act: change name
            var newName = Guid.NewGuid().ToString();
            areaType.Name = newName;
            _areaTypeRepository.SaveAreaType(areaType);

            // Assert: name change in DB
            Assert.AreEqual(newName, _areaTypeRepository.GetAreaType(id).Name);
        }

        [TestMethod]
        public void TestSaveAreaType_Replace_Components()
        {
            var id = AreaTypeIds.CountyQuintile;
            var areaType = _areaTypeRepository.GetAreaType(id);

            // Act: Save components
            SetComponentAreaTypesString(areaType, "1,2");

            // Act: Replace components
            areaType = _areaTypeRepository.GetAreaType(id);
            SetComponentAreaTypesString(areaType, "2,3");

            // Assert: Component as expected
            var components = _areaTypeRepository.GetAreaType(id).ComponentAreaTypes;
            Assert.AreEqual(2, components.ElementAt(0).ComponentAreaTypeId);
            Assert.AreEqual(3, components.ElementAt(1).ComponentAreaTypeId);
        }

        [TestMethod]
        public void TestSaveAreaType_Remove_Components()
        {
            var id = AreaTypeIds.CountyQuintile;
            var areaType = _areaTypeRepository.GetAreaType(id);

            // Act: Save components
            SetComponentAreaTypesString(areaType, "1,2");

            // Act: Remove components
            areaType = _areaTypeRepository.GetAreaType(id);
            SetComponentAreaTypesString(areaType, "");

            // Assert: Component as expected
            Assert.AreEqual(0, _areaTypeRepository.GetAreaType(id).ComponentAreaTypes.Count);

        }

        private void SetComponentAreaTypesString(AreaType areaType, string areaTypes)
        {
            areaType.ComponentAreaTypesString = areaTypes;
            areaType.ParseComponentAreaTypesString();
            _areaTypeRepository.SaveAreaType(areaType);
        }
    }
}

