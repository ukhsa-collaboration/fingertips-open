using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaTypeFactoryTest
    {
        [TestMethod]
        public void NewAreaType()
        {
            var id = AreaTypeIds.Ccg;
            var areaType = AreaTypeFactory.New(ReaderFactory.GetAreasReader(), new ParentAreaGroup { ParentAreaTypeId = id });
            Assert.AreEqual(id, areaType.Id);
        }

        [TestMethod]
        public void NewCategoryAreaType()
        {
            var id = CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority;
            var areaType = AreaTypeFactory.New(ReaderFactory.GetAreasReader(), new ParentAreaGroup { CategoryTypeId = id });
            Assert.IsNotNull(areaType);
        }

        [TestMethod]
        public void NewCategoryAreaType_NameIsDefined()
        {
            var name = "deprivation decile";
            var id = CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority;
            var areaType = AreaTypeFactory.New(ReaderFactory.GetAreasReader(), new ParentAreaGroup { CategoryTypeId = id });

            Assert.IsTrue(areaType.Name.ToLower().Contains(name));
            Assert.IsTrue(areaType.ShortName.ToLower().Contains(name));
        }

        [TestMethod]
        public void NewAreaTypeFromCategoryAreaTypeId()
        {
            var areaTypeId = CategoryAreaType.GetAreaTypeIdFromCategoryTypeId(CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority);
            var areaType = AreaTypeFactory.New(ReaderFactory.GetAreasReader(), areaTypeId);
            Assert.AreEqual(areaTypeId, areaType.Id);
        }

        [TestMethod]
        public void NewAreaTypeFromStandardAreaTypeId()
        {
            var areaTypeId = AreaTypeIds.Ccg;
            var areaType = AreaTypeFactory.New(ReaderFactory.GetAreasReader(), areaTypeId);
            Assert.AreEqual(areaTypeId, areaType.Id);
        }
    }
}
