
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    /// <summary>
    /// NOTE: 
    /// Further tests of ComparatorMap are in ComparatorMapBuilderTest
    /// </summary>
    [TestClass]
    public class ComparatorMapTest
    {
        [TestMethod]
        public void TestCannotAddNullComparator()
        {
            ComparatorMap map = new ComparatorMap();
            map.Add(null);
            Assert.AreEqual(0, map.Count);
        }
    }
}
