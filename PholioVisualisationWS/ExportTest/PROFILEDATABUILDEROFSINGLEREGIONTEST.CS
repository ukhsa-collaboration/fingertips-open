using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class ProfileDataBuilderOfSingleRegionTest
    {
        private static IWorkbook workbook;

        [ClassInitialize]
        public static void RunOnceBeforeAllTests(TestContext testContext)
        {
            InitWorksheets();
        }

        private static void InitWorksheets()
        {
            workbook = GetWorkbookForProfile(ProfileIds.Phof);
        }

        public static IWorkbook GetWorkbookForProfile(int profileId)
        {
            var region = ReaderFactory.GetAreasReader().GetAreaFromCode(AreaCodes.Gor_EastOfEngland);
            var parentArea = new ParentArea(region.Code, AreaTypeIds.CountyAndUnitaryAuthority);
            IList<ParentArea> parentAreas = new List<ParentArea>();
            parentAreas.Add(parentArea);

            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            var parentAreaTypeId = AreaTypeIds.GoRegion;

            return new ProfileDataBuilder(map, profile, new List<int> { profileId }, ParentDisplay.NationalAndRegional,
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)).BuildWorkbook();
        }

        [TestMethod]
        public void TestSubnationalSheetExists()
        {
            Assert.IsNotNull(RegionWorksheet());
        }

        [TestMethod]
        public void TestWorksheetName()
        {
            Assert.AreEqual("Region", RegionWorksheet().Name);
        }

        [TestMethod]
        public void TestAreaCodesInRegionWorksheet()
        {
            var cells = RegionWorksheet().Cells;
            var parentAreaCodeCell1 = cells[1, ExcelColumnIndexes.AreaCode];
            var parentAreaCodeCell2 = cells[2, ExcelColumnIndexes.AreaCode];

            Assert.IsFalse(ExportTestHelper.IsCellEmpty(parentAreaCodeCell1), "No area code defined");
            Assert.AreEqual(parentAreaCodeCell1.Value, parentAreaCodeCell2.Value);
        }

        private static IWorksheet RegionWorksheet()
        {
            return workbook.Worksheets["Region"];
        }
    }
}
