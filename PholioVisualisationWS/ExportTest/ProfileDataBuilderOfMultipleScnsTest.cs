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
    public class ProfileDataBuilderOfMultipleScnsTest
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
            var parentArea = new ParentArea(region.Code, AreaTypeIds.StrategicClinicalNetwork);
            IList<ParentArea> parentAreas = new List<ParentArea>();
            parentAreas.Add(parentArea);

            var map = new ComparatorMapBuilder(parentAreas).ComparatorMap;
            var profileId = ProfileIds.CardioVascularDisease;
            var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
            var parentAreaTypeId = AreaTypeIds.Country;

            workbook = new ProfileDataBuilder(map, profile, new List<int> { profileId }, ParentDisplay.NationalAndRegional,
                parentAreas, AreaTypeFactory.New(ReaderFactory.GetAreasReader(), parentAreaTypeId)
                ).BuildWorkbook();
        }

        [TestMethod]
        public void TestOnlyOneSheetContainsNationalData()
        {
            var worksheets = workbook.Worksheets;
            Assert.IsNotNull(worksheets["England"]);
            Assert.IsNull(worksheets["Country"]);
        }

    }
}
