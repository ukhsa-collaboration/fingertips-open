using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class SparklineRootBuilderTest
    {
        [TestMethod]
        public void TestBuildRoots()
        {
            var roots = new SparklineRootBuilder().BuildRoots("EMSHA",
                AreaTypeIds.Pct,ProfileIds.HealthInequalities, GroupIds.HealthInequalities,
                new List<string>{"80_L_int_","1_L_int_","val"},SexIds.Persons);

            Assert.IsTrue(roots.Count > 0);
        }
    }
}
