using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class CoreDataCollectorTest
    {
        [TestMethod]
        public void Test_Single_Area_Data()
        {
            var collector = new CoreDataCollector();

            collector.AddData(new CoreDataSet());
            collector.AddData(new CoreDataSet());
            collector.AddData(new CoreDataSet());

            Assert.AreEqual(3, collector.GetDataList().Count);
        }

        [TestMethod]
        public void Test_Multiple_Area_Data_By_Area_Code()
        {
            var collector = new CoreDataCollector();

            collector.AddDataList(GetDataThatVariesByAreaCode());
            collector.AddDataList(GetDataThatVariesByAreaCode());

            Assert.AreEqual(2, collector.GetDataListForArea(new CategoryIdAndAreaCode {AreaCode = "a"}).Count);
        }

        [TestMethod]
        public void Test_Multiple_Area_Data_By_Category_Id()
        {
            var collector = new CoreDataCollector();

            collector.AddDataList(GetDataThatVariesByCategoryId());
            collector.AddDataList(GetDataThatVariesByCategoryId());

            Assert.AreEqual(2, collector.GetDataListForArea(new CategoryIdAndAreaCode { CategoryId = 2}).Count);
        }

        private IList<CoreDataSet> GetDataThatVariesByAreaCode()
        {
            return new List<CoreDataSet>
            {
                new CoreDataSet {AreaCode = "a"},
                new CoreDataSet {AreaCode = "b"},
                new CoreDataSet {AreaCode = "c"}
            };
        }

        private IList<CoreDataSet> GetDataThatVariesByCategoryId()
        {
            return new List<CoreDataSet>
            {
                new CoreDataSet {CategoryId = 1},
                new CoreDataSet {CategoryId = 2},
                new CoreDataSet {CategoryId = 3}
            };
        }
    }
}
