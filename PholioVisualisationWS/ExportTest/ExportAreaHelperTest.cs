using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class ExportAreaHelperTest
    {
        private ExportAreaHelper _areaHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                ParentAreaCode = AreaCodes.England,
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                ProfileId = ProfileIds.Phof
            };

            InitAreaHelper(parameters);
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

            InitAreaHelper(parameters);

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

        private void InitAreaHelper(IndicatorExportParameters parameters)
        {
            var areasReader = ReaderFactory.GetAreasReader();
            _areaHelper = new ExportAreaHelper(areasReader, parameters, new AreaFactory(areasReader));
            _areaHelper.Init();
        }
    }
}
