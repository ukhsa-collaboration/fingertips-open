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
    public class ProfileDataBuilderOfMultipleRegionsTest
    {
        private static IWorkbook workbook;

        [ClassInitialize]
        public static void RunOnceBeforeAllTests(TestContext testContext)
        {
            InitWorksheets();
        }

        private static void InitWorksheets()
        {
            var region = ReaderFactory.GetAreasReader().GetAreaFromCode(AreaCodes.England);
            var parentArea = new ParentArea(region.Code, AreaTypeIds.CountyAndUnitaryAuthority);
            IList<ParentArea> parentAreas = new List<ParentArea>();
            parentAreas.Add(parentArea);

            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profileId = ProfileIds.Phof;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            var parentAreaTypeId = AreaTypeIds.GoRegion;

            workbook = new ProfileDataBuilder(map, profile, new List<int> { profileId }, ParentDisplay.NationalAndRegional,
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)
                ).BuildWorkbook();
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
        public void TestAreaCodesInRegionSheet()
        {
            var cells = RegionWorksheet().Cells;
            var parentAreaCode1 = (string)cells[1, ExcelColumnIndexes.AreaCode].Value;
            var parentAreaCode2 = (string)cells[2, ExcelColumnIndexes.AreaCode].Value;

            Assert.IsFalse(string.IsNullOrWhiteSpace(parentAreaCode1));
            Assert.IsFalse(string.IsNullOrWhiteSpace(parentAreaCode2));
            Assert.AreNotEqual(parentAreaCode1, parentAreaCode2,"Expect region area codes to be different");
        }

        private static IWorksheet RegionWorksheet()
        {
            return workbook.Worksheets["Region"];
        }
    }
}
