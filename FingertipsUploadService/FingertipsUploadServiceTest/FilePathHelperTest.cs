using FingertipsUploadService.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class FilePathHelperTest
    {
        [TestMethod]
        public void TestNewExcelFilePath()
        {
            const string currentPath = @"c:\temp\new.csv";
            const string newExpectedPath = @"c:\temp\new.xls";

            var newPath = FilePathHelper.NewExcelFilePath(currentPath);
            Assert.AreEqual(newExpectedPath, newPath);
        }
    }
}
