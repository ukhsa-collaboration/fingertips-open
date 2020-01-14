using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.ServicesWeb.Validations;

namespace PholioVisualisation.ServicesWebTest.Validations
{
    [TestClass]
    public class DataInternalServiceValidatorTest
    {
        [TestMethod]
        public void ShouldNotHaveExceptionsLatestDataFileForGroup()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "test", "1", "1", null, "", null, null, 1);
            var resultList = dataInternalServiceValidator.ValidateLatestDataFileForGroup();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ShouldHaveExceptionsLatestDataFileForGroup()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "", "1", "1", null, "");
            var resultList = dataInternalServiceValidator.ValidateLatestDataFileForGroup();

            Assert.IsTrue(resultList.Count == 2);
        }

        [TestMethod]
        public void ShouldNotHaveExceptionsLatestDataFileForIndicator()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "test", "1", "1", "test", "test", null, 1);
            var resultList = dataInternalServiceValidator.ValidateLatestDataFileForIndicator();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ShouldHaveExceptionsLatestDataFileForIndicator()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "", "1", "1", null, "");
            var resultList = dataInternalServiceValidator.ValidateLatestDataFileForIndicator();

            Assert.IsTrue(resultList.Count == 3);
        }
        [TestMethod]
        public void ShouldNotHaveExceptionsAllPeriodsWithInequalitiesDataFileForIndicator()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "test", "1", "1", "test", "", "", 1, null, "test");
            var resultList = dataInternalServiceValidator.ValidateAllPeriodsWithInequalitiesDataFileForIndicator();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ShouldHaveExceptionsAllPeriodsWithInequalitiesDataFileForIndicator()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "", "1", "1");
            var resultList = dataInternalServiceValidator.ValidateAllPeriodsWithInequalitiesDataFileForIndicator();

            Assert.IsTrue(resultList.Count == 4);
        }

        [TestMethod]
        public void ShouldNotHaveExceptionsAllPeriodDataFileByIndicator()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "", "1", "1");
            var resultList = dataInternalServiceValidator.ValidateAllPeriodDataFileByIndicator();

            Assert.IsTrue(resultList.Count == 4);
        }

        [TestMethod]
        public void ShouldHaveExceptionsAllPeriodDataFileByIndicator()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "test", "1", "1", "test", "test", null, 1);
            var resultList = dataInternalServiceValidator.ValidateAllPeriodDataFileByIndicator();

            Assert.IsTrue(resultList.Count == 0);
        }

        [TestMethod]
        public void ShouldHaveExceptionsLatestPopulationDataFile()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "", "1", "1");
            var resultList = dataInternalServiceValidator.ValidateLatestPopulationDataFile();

            Assert.IsTrue(resultList.Count == 2);
        }

        [TestMethod]
        public void ShouldNotHaveExceptionsLatestPopulationDataFile()
        {
            var dataInternalServiceValidator = new DataInternalServiceValidator(1, 1, "test", "1", "1", null, "test");
            var resultList = dataInternalServiceValidator.ValidateLatestPopulationDataFile();

            Assert.IsTrue(resultList.Count == 0);
        }
    }
}
