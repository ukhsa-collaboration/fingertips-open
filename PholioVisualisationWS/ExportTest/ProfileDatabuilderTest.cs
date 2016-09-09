using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class ProfileDatabuilderTest
    {
        [TestMethod]
        public void TestMetadataIsNotDuplicatedIfIIndicatorAppearInMoreThanOneDomain()
        {
            // Health Profiles contains "All indicators" domain so every indicator
            // is duplicated amongst the domains
            var workbook = ProfileDataBuilderOfSingleRegionTest.GetWorkbookForProfile(
                ProfileIds.HealthProfiles);

            var cells = IndicatorMetadataWorksheet(workbook).Cells;
            var indicatorNames = new List<string>();
            for (int rowIndex = 1; rowIndex < 100; rowIndex++)
            {
                var name = cells[rowIndex, ExcelColumnIndexes.IndicatorName].Value;
                if (name == null || string.IsNullOrWhiteSpace(name.ToString()))
                {
                    break;
                }
                indicatorNames.Add(name.ToString());
            }

            Assert.IsTrue(indicatorNames.Count > 0);

            Assert.AreEqual(indicatorNames.Count,
                indicatorNames.Distinct().Count(), "Some indicator names are duplicated");
        }

        [TestMethod]
        public void TestCoreDataIsNotDuplicatedIfIIndicatorAppearInMoreThanOneDomain()
        {
            var workbook = ProfileDataBuilderOfSingleRegionTest.GetWorkbookForProfile(
                ProfileIds.SexualHealth);

            var cells = workbook.Worksheets[ProfileDataBuilder.NationalLabel].Cells;
            var lastData = string.Empty;
            var dataList = new List<string>();
            for (int rowIndex = 1; rowIndex < 1000; rowIndex++)
            {
                var name = cells[rowIndex, ExcelColumnIndexes.IndicatorName].Value;
                var sex = cells[rowIndex, ExcelColumnIndexes.Sex].Value;
                var age = cells[rowIndex, ExcelColumnIndexes.Age].Value;

                var data = name + "-" + sex + "-" + age;
                if (data != lastData)
                {
                    dataList.Add(data);
                    lastData = data;
                }
            }

            Assert.IsTrue(dataList.Count > 0);

            Assert.AreEqual(dataList.Count,
                dataList.Distinct().Count(), "Some core data rows are duplicated");
        }

        private static IWorksheet IndicatorMetadataWorksheet(IWorkbook workbook)
        {
            return workbook.Worksheets["Indicator Metadata"];
        }

    }
}
