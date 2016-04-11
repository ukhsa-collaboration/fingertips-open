using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class SheetNamerTest
    {
        [TestMethod]
        public void TestUniqueNamesAreProvided()
        {
            SheetNamer namer = new SheetNamer();

            Assert.AreEqual("a", namer.GetSheetName("a"));
            Assert.AreEqual("a (2)", namer.GetSheetName("a"));
            Assert.AreEqual("a (3)", namer.GetSheetName("a"));
            Assert.AreEqual("a (4)", namer.GetSheetName("a"));
        }

        [TestMethod]
        public void TestNamesDoNotExceedValidLength()
        {
            SheetNamer namer = new SheetNamer();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < SheetNamer.MaximumLength; i++)
            {
                sb.Append("a");
            }
            string valid = sb.ToString();
            AssertNameOk(namer, "aaaaaaaaaaaaaaaaaaaaaaa...", valid);
            AssertNameOk(namer, "aaaaaaaaaaaaaaaaaaaaaaa... (2)", valid);
            AssertNameOk(namer, "aaaaaaaaaaaaaaaaaaaaaaa... (3)", valid);
            AssertNameOk(namer, "aaaaaaaaaaaaaaaaaaaaaaa... (4)", valid);

            // Assert maximum available space is used
            Assert.IsTrue(namer.GetSheetName(valid).Length == SheetNamer.MaximumLength);
        }

        [TestMethod]
        public void TestSpecialCharactersAreRemoved()
        {
            var chars = new[] { '[', ']', '*', '/', ':', '?', '\\' };

            foreach (var c in chars)
            {
                SheetNamer namer = new SheetNamer();
                Assert.AreEqual("a b", namer.GetSheetName("a" + c + "b"));
            }
        }

        private void AssertNameOk(SheetNamer namer, string expected, string name)
        {
            string validName = namer.GetSheetName(name);
            Assert.AreEqual(expected, validName);
            Assert.IsTrue(validName.Length <= SheetNamer.MaximumLength);
        }
    }
}
