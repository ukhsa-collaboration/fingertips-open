using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class ChildAreasForPdfsActionTest
    {
        private const string parentCode = AreaCodes.Gor_SouthEast;
        private const int areaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;

        [TestMethod]
        public void TestGetData_BenchmarkArea()
        {
            var data = new ChildAreasForPdfsAction().GetResponse(ProfileIds.Phof, parentCode, areaTypeId);

            Assert.AreEqual(data.Benchmark.Code, AreaCodes.England);
        }


        [TestMethod]
        public void TestGetData_ParentArea()
        {
            var data = new ChildAreasForPdfsAction().GetResponse(ProfileIds.Phof,
                parentCode, areaTypeId);

            Assert.AreEqual(data.Parent.Code, parentCode);
        }

        [TestMethod]
        public void TestGetData_ChildAreas()
        {
            var data = new ChildAreasForPdfsAction().GetResponse(ProfileIds.Phof,
                parentCode, areaTypeId);

            foreach (var childArea in data.Children)
            {
                if (childArea.Name.Contains("Kent"))
                {
                    return;
                }
            }

            Assert.Fail("Expected child area not found");
        }
    }
}
