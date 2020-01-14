using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.ServicesWeb.Validations;

namespace PholioVisualisation.ServicesWebTest.Validations
{
    [TestClass]
    public class DataInternalServicesParametersTest
    {
        [TestMethod]
        public void  ShouldGetAChildAreasCodeList()
        {
            var dataInternalServicesParameters = new DataInternalServicesParameters(DataInternalServiceUse.AllPeriodDataFileByIndicator, 1, 1, "1", "1", "", null, "areaTest1,areaTest2,areaTest3");

            var result = dataInternalServicesParameters.GetChildAreasCodeList();

            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result[0] == "areaTest1");
            Assert.IsTrue(result[1] == "areaTest2");
            Assert.IsTrue(result[2] == "areaTest3");
        }

        [TestMethod]
        public void ShouldBeTheSameDataInternalServiceCalled()
        {
            var expectedResult = DataInternalServiceUse.AllPeriodDataFileByIndicator;

            var dataInternalServiceParameters = new DataInternalServicesParameters(DataInternalServiceUse.AllPeriodDataFileByIndicator, 1, 1, "1", "1", "");
            var result = dataInternalServiceParameters.GetDataInternalServiceCalled();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
