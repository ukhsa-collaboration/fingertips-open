using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.ServicesWeb.Validations;

namespace PholioVisualisation.ServicesWebTest.Validations
{
    [TestClass]
    public class DataServiceValidatorTest
    {
        [TestMethod]
        public void ShouldNotHaveWExceptionsAllDataFileForIndicatorList()
        {
            var dataServiceValidator = new DataServiceValidator(1, 1, "test", "1", "1", "test", null, null, 1);
            var resultList = dataServiceValidator.ValidateAllDataFileForIndicatorList();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ShouldHaveWExceptionsAllDataFileForIndicatorList()
        {
            var dataServiceValidator = new DataServiceValidator(1, 1, "", "1", "1");
            var resultList = dataServiceValidator.ValidateAllDataFileForIndicatorList();

            Assert.IsTrue(resultList.Count == 3);
        }

        [TestMethod]
        public void ShouldNotHaveWExceptionsAllDataFileForGroup()
        {
            var dataServiceValidator = new DataServiceValidator(1, 1, "test", "1", "1", null, null, null, null, 1);
            var resultList = dataServiceValidator.ValidateAllDataFileForGroup();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ShouldHaveWExceptionsAllDataFileForGroup()
        {
            var dataServiceValidator = new DataServiceValidator(1, 1, "", "1", "1");
            var resultList = dataServiceValidator.ValidateAllDataFileForGroup();

            Assert.IsTrue(resultList.Count == 2);
        }

        [TestMethod]
        public void ShouldNotHaveValidateAllDataFileForProfile()
        {
            var dataServiceValidator = new DataServiceValidator(1, 1, "test", "1", "1", null, null, null, 1);
            var resultList = dataServiceValidator.ValidateAllDataFileForProfile();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ShouldHaveValidateAllDataFileForProfile()
        {
            var dataServiceValidator = new DataServiceValidator(1, 1, "", "1", "1");
            var resultList = dataServiceValidator.ValidateAllDataFileForProfile();

            Assert.IsTrue(resultList.Count == 2);
        }
    }
}
