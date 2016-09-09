using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class ProfileDataBuilderOfMultipleDeprivationDecilesTest
    {
        private static IWorkbook _workbook;

        [ClassInitialize]
        public static void RunOnceBeforeAllTests(TestContext testContext)
        {
            InitDeprivationDecilesForAllEnglandWorksheets();
        }

        [TestMethod]
        public void TestCreatesParentAreaWorksheet()
        {
            Assert.IsTrue(DeprivationDecileWorksheet().Name.Contains("Deprivation decile"));
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
            var cells = DistrictUAWorksheet().Cells;
            Assert.IsNotNull(cells[1, ExcelColumnIndexes.ParentAreaCode].Value);
            Assert.IsNotNull(cells[1, ExcelColumnIndexes.ParentAreaName].Value);
        }

        private static void InitDeprivationDecilesForAllEnglandWorksheets()
        {
            // Parameters
            var profileId = ProfileIds.Phof;
            var childAreaType = AreaTypeIds.DistrictAndUnitaryAuthority;
            var parentArea = new ParentArea(AreaCodes.England, childAreaType);
            var parentAreaTypeId =
                CategoryAreaType.GetAreaTypeIdFromCategoryTypeId(
                    CategoryTypeIds.DeprivationDecileDistrictAndUA2015);

            // Create workbook
            IList<ParentArea> parentAreas = new List<ParentArea>();
            parentAreas.Add(parentArea);
            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            _workbook = new ProfileDataBuilder(map, profile, new List<int> { profileId },
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)).BuildWorkbook(); ;
        }

        private static IWorksheet DeprivationDecileWorksheet()
        {
            return _workbook.Worksheets["Deprivation decile (IMD..."];
        }

        private static IWorksheet DistrictUAWorksheet()
        {
            return _workbook.Worksheets["District & UA"];
        }
    }
}
