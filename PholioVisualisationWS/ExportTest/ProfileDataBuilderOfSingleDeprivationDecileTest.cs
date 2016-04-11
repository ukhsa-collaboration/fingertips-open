using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class ProfileDataBuilderOfSingleDeprivationDecileTest
    {
        private static IWorkbook workbook;

        [ClassInitialize]
        public static void RunOnceBeforeAllTests(TestContext testContext)
        {
            InitOneDecileWorksheets();
        }

        private static void InitOneDecileWorksheets()
        {
            var category = new Category
            {
                CategoryId = 5,
                CategoryTypeId = CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority,
                Name = "",
                ShortName = ""
            };
            var categoryArea = CategoryArea.New(category);

            var parentArea = new ParentArea(categoryArea.Code, AreaTypeIds.CountyAndUnitaryAuthority);
            IList<ParentArea> parentAreas = new List<ParentArea>();
            parentAreas.Add(parentArea);

            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profileId = ProfileIds.Phof;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            var parentAreaTypeId = CategoryAreaType.GetAreaTypeIdFromCategoryTypeId(
               CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority);
            workbook = new ProfileDataBuilder(map, profile, new List<int> { profileId }, ParentDisplay.NationalAndRegional,
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)).BuildWorkbook();
        }

        [TestMethod]
        public void TestCreatesParentAreaWorksheet()
        {
            Assert.IsTrue(DeprivationDecileWorksheet().Name.Contains("Deprivation decile"));
        }

        [TestMethod]
        public void TestOnlyOneParentAreaCodeIsWrittenForSingleParentArea()
        {
            var cells = DeprivationDecileWorksheet().Cells;
            var parentAreaCode1 = cells[1, ExcelColumnIndexes.ParentAreaCode].Value;
            var parentAreaCode2 = cells[2, ExcelColumnIndexes.ParentAreaCode].Value;
            Assert.AreEqual(parentAreaCode1, parentAreaCode2);
        }

        private static IWorksheet DeprivationDecileWorksheet()
        {
            return workbook.Worksheets["Deprivation decile (IMD..."];
        }
    }
}
