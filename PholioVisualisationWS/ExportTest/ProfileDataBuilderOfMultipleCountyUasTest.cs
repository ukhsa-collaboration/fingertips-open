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
    public class ProfileDataBuilderOfMultipleCountyUasTest
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
        public void TestAreaCodesInCountyUaSheet()
        {
            var cells = CountyUaWorksheet().Cells;
            var areaCode1 = (string)cells[1, ExcelColumnIndexes.AreaCode].Value;
            var areaCode2 = (string)cells[2, ExcelColumnIndexes.AreaCode].Value;

            Assert.IsFalse(string.IsNullOrWhiteSpace(areaCode1));
            Assert.IsFalse(string.IsNullOrWhiteSpace(areaCode2));
            Assert.AreNotEqual(areaCode1, areaCode2, "Expect area codes to be different");
        }

        private static void InitWorksheets()
        {
            // Parameters
            var profileId = ProfileIds.AdultSocialCare;
            var childAreaType = AreaTypeIds.CountyAndUnitaryAuthority;
            var parentArea = new ParentArea(AreaCodes.England, childAreaType);
            var parentAreaTypeId = AreaTypeIds.GoRegion;

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

        private static IWorksheet CountyUaWorksheet()
        {
            return workbook.Worksheets[WorksheetNames.CountyUa];
        }
    }
}
