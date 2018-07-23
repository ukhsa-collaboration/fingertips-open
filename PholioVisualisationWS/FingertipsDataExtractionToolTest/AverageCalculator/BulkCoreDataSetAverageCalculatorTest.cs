using System;
using System.Collections.Generic;
using System.Linq;
using FingertipsDataExtractionTool.AverageCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.FingertipsDataExtractionToolTest.AverageCalculator
{
    [TestClass]
    public class BulkCoreDataSetAverageCalculatorTest
    {
        [TestMethod]
        public void Test_Assign_Properties_For_Category_Area()
        {
            var category = new Category
            {
                CategoryTypeId = CategoryTypeIds.EthnicGroups5,
                Id = CategoryIds.EthnicityAsian
            };
            var area = CategoryArea.New(category);

            var data = new CoreDataSet();

            BulkCoreDataSetAverageCalculator.AssignProperties(data,area);

            // Assert: properties correctly assigned
            Assert.AreEqual(CategoryTypeIds.EthnicGroups5, data.CategoryTypeId);
            Assert.AreEqual(CategoryIds.EthnicityAsian, data.CategoryId);
            Assert.AreEqual(AreaCodes.England, data.AreaCode);
        }

        [TestMethod]
        public void Test_Assign_Properties_For_Standard_Area()
        {
            var area = new Area
            {
                Code = AreaCodes.Ccg_Barnet
            };

            var data = new CoreDataSet();

            BulkCoreDataSetAverageCalculator.AssignProperties(data, area);

            // Assert: properties correctly assigned
            Assert.AreEqual(CategoryTypeIds.Undefined, data.CategoryTypeId);
            Assert.AreEqual(CategoryIds.Undefined, data.CategoryId);
            Assert.AreEqual(AreaCodes.Ccg_Barnet, data.AreaCode);
        }
    }
}
