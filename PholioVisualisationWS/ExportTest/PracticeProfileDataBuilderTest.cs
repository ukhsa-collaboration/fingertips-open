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
        public const string SheetNameCcg = @"CCGs (since 4 19)";

        [TestMethod]
        public void TestAllSheetsAreCreated()
        {
            PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder()
            {
                AreaCode = AreaCodes.Ccg_Barnet,
                GroupIds = new List<int>{GroupIds.Population},
                ParentAreaTypeId = AreaTypeIds.CcgsPostApr2019
            };

            var workBook = builder.BuildWorkbook();
            var worksheets = workBook.Worksheets;

            Assert.AreEqual(4, worksheets.Count);
            Assert.IsNotNull(worksheets[SheetNamePractice]);
            Assert.IsNotNull(worksheets[SheetNameCcg]);
            Assert.IsNotNull(worksheets["Indicator Metadata"]);
            Assert.IsNotNull(worksheets["Practice Addresses"]);
        }

    }
}
