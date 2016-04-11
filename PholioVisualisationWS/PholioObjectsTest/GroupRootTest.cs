
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class GroupRootTest
    {
        [TestMethod]
        public void TestGetGroupings()
        {
            // Subnational
            GroupRoot root = new GroupRoot()
            {
                Grouping = new List<Grouping> {
                new Grouping{ComparatorId = ComparatorIds.Subnational}
            }};
            Assert.IsNotNull(root.FirstGrouping);

            // National
            root = new GroupRoot()
            {
                Grouping = new List<Grouping> {
                new Grouping{ComparatorId = ComparatorIds.England}
            }
            };
            Assert.IsNotNull(root.FirstGrouping);
        }
    }
}
