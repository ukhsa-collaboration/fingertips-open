using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace ExportTest
{
    [TestClass]
    public class ProfileDataBuilderOfMidlandsAndEastTest
    {
        private static IWorkbook workbook;

        [ClassInitialize]
        public static void RunOnceBeforeAllTests(TestContext testContext)
        {
            InitWorksheets();
        }

        private static void InitWorksheets()
        {
            var parentAreas = GetParentAreas();

            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profileId = ProfileIds.SubstanceMisuse;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            var parentAreaTypeId = AreaTypeIds.GoRegion;

            workbook = new ProfileDataBuilder(map, profile, new List<int> { profileId }, ParentDisplay.RegionalOnly,
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)
                ).BuildWorkbook();
        }

        [TestMethod]
        public void TestSubnationalSheetsExists()
        {
            Assert.IsNotNull(RegionWorksheet());
            Assert.IsNotNull(workbook.Worksheets["Region (2)"]);
            Assert.IsNotNull(workbook.Worksheets["Region (3)"]);
        }

        [TestMethod]
        public void TestEnglandSheetDoesNotExist()
        {
            Assert.IsNull(workbook.Worksheets[ProfileDataBuilder.NationalLabel]);
        }

        private static IList<ParentArea> GetParentAreas()
        {
            IList<ParentArea> parentAreas = new List<ParentArea>();

            var codes = new string[]
            {
                AreaCodes.Gor_EastOfEngland,
                AreaCodes.Gor_EastMidlands,
                AreaCodes.Gor_WestMidlands
            };

            foreach (var code in codes)
            {
                var parentArea = new ParentArea(code,
                    AreaTypeIds.CountyAndUnitaryAuthority);
                parentAreas.Add(parentArea);
            }

            return parentAreas;
        }

        private static IWorksheet RegionWorksheet()
        {
            return workbook.Worksheets["Region"];
        }
    }
}
