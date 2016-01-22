using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace ExportTest
{
    [TestClass]
    public class ProfileDataBuilderOfMultipleDeprivationDecilesTest
    {
        private static IWorkbook workbook;

        [ClassInitialize]
        public static void RunOnceBeforeAllTests(TestContext testContext)
        {
            InitDeprivationDecilesForAllEnglandWorksheets();
        }

        private static void InitDeprivationDecilesForAllEnglandWorksheets()
        {
            var parentArea = new ParentArea(AreaCodes.England, AreaTypeIds.CountyAndUnitaryAuthority);
            IList<ParentArea> parentAreas = new List<ParentArea>();
            parentAreas.Add(parentArea);

            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profileId = ProfileIds.Phof;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            var parentAreaTypeId =
                CategoryAreaType.GetAreaTypeIdFromCategoryTypeId(
                    CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority);
            workbook = new ProfileDataBuilder(map, profile, new List<int> { profileId }, ParentDisplay.NationalAndRegional,
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)).BuildWorkbook();;
        }

        [TestMethod]
        public void TestCreatesParentAreaWorksheet()
        {
            Assert.AreEqual("Deprivation decile", DeprivationDecileWorksheet().Name);
        }

        [TestMethod]
        public void TestDeprivationDecileCodesAreWrittenInOrderForAllEnglandData()
        {
            IRange cells = DeprivationDecileWorksheet().Cells;
            Assert.AreEqual((double)1, cells[1, ExcelColumnIndexes.AreaCode].Value);
            Assert.AreEqual((double)2, cells[2, ExcelColumnIndexes.AreaCode].Value);
        }

        [TestMethod]
        public void TestParentAreaColumnsAreEmptyDeprivationDecileWorksheet()
        {
            var cells = DeprivationDecileWorksheet().Cells;
            Assert.IsTrue(ExportTestHelper.IsCellEmpty(cells[1, ExcelColumnIndexes.ParentAreaCode]));
            Assert.IsTrue(ExportTestHelper.IsCellEmpty(cells[1, ExcelColumnIndexes.ParentAreaName]));
        }

        [TestMethod]
        public void TestDeprivationDecileCodesAreWrittenForParentAreas()
        {
            var cells = CountyUAWorksheet().Cells;
            Assert.IsNotNull(cells[1, ExcelColumnIndexes.ParentAreaCode].Value);
            Assert.IsNotNull(cells[1, ExcelColumnIndexes.ParentAreaName].Value);
        }

        private static IWorksheet DeprivationDecileWorksheet()
        {
            return workbook.Worksheets["Deprivation decile"];
        }

        private static IWorksheet CountyUAWorksheet()
        {
            return workbook.Worksheets["County & UA"];
        }
    }
}
