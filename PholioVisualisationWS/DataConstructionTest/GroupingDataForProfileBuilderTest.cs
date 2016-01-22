using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class GroupingDataForProfileBuilderTest
    {
        [TestMethod]
        public void TestBuild()
        {
            IList<GroupRootSummary> data = new GroupRootSummaryBuilder()
                .Build(ProfileIds.Phof, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }
    }
}