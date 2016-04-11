
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class TrendDataParametersTest
    {
        [TestMethod]
        public void AreValid()
        {
            TrendDataParameters parameters = new TrendDataParameters(GetValidParameters());
            Assert.IsTrue(parameters.AreValid);
        }

        private static NameValueCollection GetValidParameters()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.GroupIds, "1");
            nameValues.Add(ParameterNames.ParentAreaCode, AreaCodes.Sha_EastOfEngland);
            nameValues.Add(ParameterNames.AreaTypeId, "2");
            nameValues.Add(ParameterNames.ProfileId, "1");
            nameValues.Add("jsonp_callback", "a");
            return nameValues;
        }

    }
}
