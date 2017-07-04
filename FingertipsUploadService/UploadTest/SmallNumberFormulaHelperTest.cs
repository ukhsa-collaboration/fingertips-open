using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class SmallNumberFormulaHelperTest
    {
        private SmallNumberFormulaHelper _helper;

        [TestInitialize]
        public void Setup()
        {
            _helper = new SmallNumberFormulaHelper();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _helper = null;
        }

        [TestMethod]
        public void CorrectValueTest()
        {
            var result = _helper.IsSmallNumber(1, "($ >=0)  AND  ($ <5)");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LessThanZeroValueTest()
        {
            var result = _helper.IsSmallNumber(-1, "($ >=0)  AND  ($ <5)");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BiggerThanMaxValueTest()
        {
            var result = _helper.IsSmallNumber(7, "($ >=0)  AND  ($ <5)");
            Assert.IsFalse(result);
        }
    }
}
