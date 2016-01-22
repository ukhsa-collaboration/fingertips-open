
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.RequestParameters;

namespace RequestParametersTest
{
    [TestClass]
    public class TrendDataBySearchParametersTest
    {
        [TestMethod]
        public void TestAreValid()
        {
            Assert.IsTrue(new TrendDataBySearchParameters(GetNameValues()).AreValid);
        }

        private NameValueCollection GetNameValues()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(TrendDataBySearchParameters.ParameterIndicatorIds, "2");
            nameValues.Add(ParameterNames.ParentAreaCode, "a");
            nameValues.Add(ParameterNames.AreaTypeId, "5");
            return nameValues;
        }
    }
}
