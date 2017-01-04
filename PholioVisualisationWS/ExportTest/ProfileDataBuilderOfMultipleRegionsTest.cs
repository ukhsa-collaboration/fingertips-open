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
    public class ProfileDataBuilderOfMultipleRegionsTest
    {
        private static IWorkbook workbook;

        [ClassInitialize]
        public static void RunOnceBeforeAllTests(TestContext testContext)
        {
            InitWorksheets();
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
            Assert.AreNotEqual(parentAreaCode1, parentAreaCode2, "Expect region area codes to be different");
        }

        private static void InitWorksheets()
        {
            // Parameters
            var profileId = ProfileIds.Phof;
            var childAreaType = AreaTypeIds.GoRegion;
            var parentArea = new ParentArea(AreaCodes.England, childAreaType);
            var parentAreaTypeId = AreaTypeIds.Country;

            // Create workbook
            IList<ParentArea> parentAreas = new List<ParentArea>();
            parentAreas.Add(parentArea);
            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            workbook = new ProfileDataBuilder(map, profile, new List<int> { profileId },
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)
                ).BuildWorkbook();
        }

        private static IWorksheet RegionWorksheet()
        {
            return workbook.Worksheets[WorksheetNames.Region];
        }
    }
}
