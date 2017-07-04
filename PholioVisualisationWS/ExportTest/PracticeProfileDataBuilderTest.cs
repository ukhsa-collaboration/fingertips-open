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
        public const string SheetNameCcg = "CCG";

        [TestMethod]
        public void TestAllSheetsAreCreated()
        {
            PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder()
            {
                AreaCode = AreaCodes.Ccg_Barnet,
                GroupIds = new List<int>{GroupIds.Population},
                ParentAreaTypeId = AreaTypeIds.Ccg
            };

            var workBook = builder.BuildWorkbook();
            Assert.AreEqual(4, workBook.Worksheets.Count);
            Assert.IsNotNull(workBook.Worksheets[SheetNamePractice]);
            Assert.IsNotNull(workBook.Worksheets[SheetNameCcg]);
            Assert.IsNotNull(workBook.Worksheets["Indicator Metadata"]);
            Assert.IsNotNull(workBook.Worksheets["Practice Addresses"]);
        }

    }
}
