using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class AreaAddressProviderTest
    {
        private AreaAddressProvider _areaAddressProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            _areaAddressProvider = new AreaAddressProvider(areasReader);
        }

        [TestMethod]
        public void Test_Ccg()
        {
            var code = AreaCodes.Ccg_Barnet;
            var area = _areaAddressProvider.GetAreaAddress(AreaCodes.Ccg_Barnet);
            Assert.AreEqual(code, area.Code);
        }

        [TestMethod]
        public void Test_CategoryArea()
        {
            var code = CategoryArea.CreateAreaCode(1,1);
            var area = _areaAddressProvider.GetAreaAddress(code);
            Assert.AreEqual(code, area.Code);
        }

        [TestMethod]
        public void Test_NearestNeighbourArea()
        {
            var code = NearestNeighbourArea.CreateAreaCode(
                NearestNeighbourTypeIds.Cipfa, AreaCodes.CountyUa_Leicestershire);
            var area = _areaAddressProvider.GetAreaAddress(code);
            Assert.AreEqual(code, area.Code);
        }
    }
}
