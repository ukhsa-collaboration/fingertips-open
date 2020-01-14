using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using System.Linq;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class ExportAreaHelperTest
    {
        private ExportAreaHelper _areaHelper;
        private IAreasReader _areaReader;

        [TestInitialize]
        public void TestInitialize()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                ParentAreaCode = AreaCodes.England,
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                ProfileId = ProfileIds.Phof
            };

            _areaReader = ReaderFactory.GetAreasReader();

            _areaHelper = CreateAreaHelper(_areaReader, parameters);
        }

        [TestMethod]
        public void TestChildAreasWhenParentIsEngland()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.Stp,
                ParentAreaCode = AreaCodes.England,
                ParentAreaTypeId = AreaTypeIds.Country,
                ProfileId = ProfileIds.MentalHealthJsna
            };

            _areaHelper  = CreateAreaHelper(_areaReader, parameters);

            Assert.IsTrue(_areaHelper.ChildAreaCodes.Contains(AreaCodes.Stp_Devon));
        }

        [TestMethod]
        public void TestEngland()
        {
            Assert.AreEqual(AreaCodes.England, _areaHelper.England.Code);
        }

        [TestMethod]
        public void TestParentAreaCodes()
        {
            Assert.IsTrue(_areaHelper.ParentAreaCodes.Contains(AreaCodes.Gor_NorthEast));
        }

        [TestMethod]
        public void TestChildAreaCodes()
        {
            Assert.IsTrue(_areaHelper.ChildAreaCodes.Contains(AreaCodes.CountyUa_Leicestershire));
        }

        [TestMethod]
        public void TestChildAreaCodeToParentAreaMap()
        {
            Assert.AreEqual(AreaCodes.Gor_EastMidlands, 
                _areaHelper.ChildAreaCodeToParentAreaMap[AreaCodes.CountyUa_Leicestershire].Code);
        }

        [TestMethod]
        public void TestParentAreaCodeToChildAreaCodesMap()
        {
            var childList = _areaHelper.ParentAreaCodeToChildAreaCodesMap[AreaCodes.Gor_EastMidlands];
            Assert.IsTrue(childList.Contains(AreaCodes.CountyUa_Leicestershire));
        }

        [TestMethod]
        public void TestIsNotCategoryParentAreaCode()
        {
            var result = _areaHelper.IsCategoryParentAreaCode();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsCategoryParentAreaCode()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                ParentAreaCode = AreaCodes.DeprivationDecile_Utla3,
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                ProfileId = ProfileIds.Phof
            };

            var areaHelper = CreateAreaHelper(_areaReader, parameters);

            var result = areaHelper.IsCategoryParentAreaCode();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestParentAreaCodeFromCategoryTypeId()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                ParentAreaCode = AreaCodes.DeprivationDecile_Utla3,
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                ProfileId = ProfileIds.Phof
            };

            var areaHelper = CreateAreaHelper(_areaReader, parameters);

            var categoriesTypeId = new[] {"cat-2-3"};
            var parentsAreaCode = areaHelper.ParentAreaCodeFromCategoryTypeId(categoriesTypeId);

            Assert.AreEqual(parentsAreaCode[0], "E92000001");
        }

        [TestMethod]
        public void TestIsNotCategoryAreaCode()
        {
            var result = ExportAreaHelper.IsCategoryAreaCode("AnyAreaCode");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsCategoryAreaCode()
        {
            var result = ExportAreaHelper.IsCategoryAreaCode(AreaCodes.DeprivationDecile_Utla3);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestIsNotCategoryAreaCodeList()
        {
            var areaCodeList = new []{ AreaCodes.England, AreaCodes.Ccg_Barnet };
            var result = ExportAreaHelper.IsCategoryAreaCode(areaCodeList);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsCategoriesAreaCodeList()
        {
            var areaCodeList = new[] { AreaCodes.England, AreaCodes.DeprivationDecile_Utla3 };
            var result = ExportAreaHelper.IsCategoryAreaCode(areaCodeList);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestIsNotAreaListCode()
        {
            var areaCode = AreaCodes.DeprivationDecile_Utla3;
            var result = ExportAreaHelper.IsAreaListCode(areaCode);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsAreaListCodeList()
        {
            var areaCode = AreaCodes.GenericAreaListCodeForTest;
            var result = ExportAreaHelper.IsAreaListCode(areaCode);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestGetInequalityFromAreaCode()
        {
            var results = ExportAreaHelper.GetCategoryInequalityFromAreaCode(AreaCodes.DeprivationDecile_Utla3, _areaReader);

            Assert.AreEqual(results.CategoryTypeId, 2);
            Assert.AreEqual(results.CategoryId, 3);
        }

        private ExportAreaHelper CreateAreaHelper(IAreasReader areaReader, IndicatorExportParameters parameters)
        {
            var areaHelper = new ExportAreaHelper(areaReader, parameters);
            areaHelper.Init();

            return areaHelper;
        }
    }
}
