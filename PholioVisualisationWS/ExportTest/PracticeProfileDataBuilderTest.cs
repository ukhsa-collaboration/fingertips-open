using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class PracticeProfileDataBuilderTest
    {
        public const string SheetNamePractice = "Practice";
        public const string SheetNameCountyUA = "County & UA";
        public const string SheetNameCcg = "CCG";

        [TestMethod]
        public void TestAllSheetsAreCreatedForCcg()
        {
            PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder(false)
            {
                AreaCode = AreaCodes.England,
                GroupIds = new List<int>{GroupIds.PracticeProfiles_Cvd},
                AreaTypeId = AreaTypeIds.Ccg
            };

            var workBook = builder.BuildWorkbook();
            Assert.AreEqual(4, workBook.Worksheets.Count);
            Assert.IsNotNull(workBook.Worksheets[SheetNamePractice]);
            Assert.IsNotNull(workBook.Worksheets[SheetNameCcg]);
            Assert.IsNotNull(workBook.Worksheets["Indicator Metadata"]);
            Assert.IsNotNull(workBook.Worksheets["Practice Addresses"]);
        }


        [TestMethod]
        public void RegressionTest_CvdHeartFailureBuilds()
        {
            PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder(false)
            {
                AreaCode = AreaCodes.England,
                GroupIds = new List<int> { GroupIds.PracticeProfiles_CvdHeartFailure },
                AreaTypeId = AreaTypeIds.Ccg
            };

            var workBook = builder.BuildWorkbook();
            Assert.AreEqual(4, workBook.Worksheets.Count);
        }

        [TestMethod]
        public void TestAllSheetsAreCreatedForCountyUa()
        {
            PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder(false)
            {
                AreaCode = AreaCodes.England,
                GroupIds = new List<int>{GroupIds.PracticeProfiles_Cvd},
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            };

            var workBook = builder.BuildWorkbook();
            Assert.AreEqual(4, workBook.Worksheets.Count);
            Assert.IsNotNull(workBook.Worksheets[SheetNamePractice]);
            Assert.IsNotNull(workBook.Worksheets[SheetNameCountyUA]);
            Assert.IsNotNull(workBook.Worksheets["Indicator Metadata"]);
            Assert.IsNotNull(workBook.Worksheets["Practice Addresses"]);
        }

        [TestMethod]
        public void TestMultipleGroupIds()
        {
            PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder(false)
            {
                AreaCode = AreaCodes.England,
                GroupIds = new List<int> { GroupIds.Diabetes_TreatmentTargets,
                    GroupIds.Diabetes_CareProcesses },
                AreaTypeId = AreaTypeIds.Ccg
            };

            var workBook = builder.BuildWorkbook();
            var cells = workBook.Worksheets[SheetNamePractice].Cells;
            var parentAreaCode1 = cells[3, 1].Value;
            var parentAreaCode2 = cells[4, 1].Value;
            Assert.AreEqual(parentAreaCode1, parentAreaCode2);
        }
    }
}
