
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupMetadataBuilderTest
    {
        [TestMethod]
        public void TestBuild()
        {
            GroupMetadataBuilder builder = new GroupMetadataBuilder(ReaderFactory.GetGroupDataReader())
            {
                GroupIds = new List<int> { 2000002, 2000003 }
            };

            var list = builder.Build();

            Assert.AreEqual(2, list.Count);
        }


        [TestMethod]
        public void TestBuildNoGroupId()
        {
            GroupMetadataBuilder builder = new GroupMetadataBuilder(ReaderFactory.GetGroupDataReader());
            Assert.AreEqual(0, builder.Build().Count);
        }
    }
}
