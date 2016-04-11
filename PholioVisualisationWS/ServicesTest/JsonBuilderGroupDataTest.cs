
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.Services;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class JsonBuilderGroupDataTest
    {
        [TestMethod]
        public void TestIncompleteDataReturnsEmptyCallback()
        {
            JsonBuilderGroupData builder = new JsonBuilderGroupData();
            AssertJsonIsEmptyCallback(builder);

            builder = new JsonBuilderGroupData();
            AssertJsonIsEmptyCallback(builder);

            builder = new JsonBuilderGroupData {IndicatorMetadata = GetIndicatorMetadata() };
            AssertJsonIsEmptyCallback(builder);

        }

        private static void AssertJsonIsEmptyCallback(JsonBuilderGroupData builder)
        {
            Assert.AreEqual("", builder.BuildJson());
        }

        [TestMethod]
        public void TestSimplestRequiredDataReturnsJson()
        {
            JsonBuilderGroupData builder = new JsonBuilderGroupData
            {
                GroupRoots = GetGroupRoots(),
                IndicatorMetadata = GetIndicatorMetadata()
            };
            Assert.IsTrue(builder.BuildJson().Length > 10);
        }

        private List<IndicatorMetadata> GetIndicatorMetadata()
        {
            return
                new List<IndicatorMetadata> { new IndicatorMetadata {
                    IndicatorId = 12, UnitId = 2, ValueTypeId = 3 }};
        }

        private IList<GroupRoot> GetGroupRoots()
        {
            return new[]{
                new GroupRoot()
            };
        }
    }
}
