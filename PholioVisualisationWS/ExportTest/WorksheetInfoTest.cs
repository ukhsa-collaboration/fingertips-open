using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;

namespace ExportTest
{
    [TestClass]
    public class WorksheetInfoTest
    {
        [TestMethod]
        public void TestNextRow()
        {
            WorksheetInfo wi = new WorksheetInfo();
            Assert.AreEqual(0, wi.NextRow);
            Assert.AreEqual(1, wi.NextRow);
            Assert.AreEqual(2, wi.NextRow);
        }

        [TestMethod]
        public void TestIsWorksheetEmpty()
        {
            WorksheetInfo wi = new WorksheetInfo();
            int row = wi.NextRow;
            Assert.IsTrue(wi.IsWorksheetEmpty);
            row = wi.NextRow;
            Assert.IsFalse(wi.IsWorksheetEmpty);
            row = wi.NextRow;
            Assert.IsFalse(wi.IsWorksheetEmpty);
        }
    }
}
