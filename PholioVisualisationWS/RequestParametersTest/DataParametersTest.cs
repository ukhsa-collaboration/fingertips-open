
using System;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class DataParametersTest
    {
        [TestMethod]
        public void TestIndicatorIdsOverridesProfileId()
        {
            // Only profile ID
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.ProfileId, "1");
            DataParameters parameters = new DataParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.UseProfile);

            // Only Indicator IDs
            nameValues = new NameValueCollection();
            nameValues.Add(DataParameters.ParameterIndicatorIds, "1");
            parameters = new DataParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.UseIndicatorIds);

            // Both together are invalid
            nameValues = new NameValueCollection();
            nameValues.Add(DataParameters.ParameterIndicatorIds, "1");
            nameValues.Add(ParameterNames.ProfileId, "1");
            parameters = new DataParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.UseIndicatorIds);
        }
    }
}
