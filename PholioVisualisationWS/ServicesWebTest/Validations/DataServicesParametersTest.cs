using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.ServicesWeb.Validations;

namespace PholioVisualisation.ServicesWebTest.Validations
{
    [TestClass]
    public class DataServicesParametersTest
    {
        [TestMethod]
        public void ShouldBeTheSameDataServiceCalled()
        {
            var expectedResult = DataServiceUse.AllDataFileForIndicatorList;

            var dataServiceParameters = new DataServicesParameters(expectedResult, 1, 1, "", "1", "1");
            var result = dataServiceParameters.GetDataServiceCalled();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
