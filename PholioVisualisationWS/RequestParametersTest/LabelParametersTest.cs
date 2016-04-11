
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class LabelParametersTest
    {
        [TestMethod]
        public void TestValidAge()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.AgeId, "12");
            nameValues.Add(LabelParameters.ParameterAjaxKey, "1");
            LabelParameters parameters = new LabelParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.IsAgeIdValid);
            Assert.AreEqual(12, parameters.AgeId);
        }

        [TestMethod]
        public void TestValidYearType()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.YearTypeId, "12");
            nameValues.Add(LabelParameters.ParameterAjaxKey, "1");
            LabelParameters parameters = new LabelParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.IsYearTypeIdValid);
            Assert.AreEqual(12, parameters.YearTypeId);
        }

        [TestMethod]
        public void TestValidComparatorMethod()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(LabelParameters.ParameterComparatorMethod, "2");
            nameValues.Add(LabelParameters.ParameterAjaxKey, "1");
            LabelParameters parameters = new LabelParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.IsComparatorMethodIdValid);
            Assert.AreEqual(2, parameters.ComparatorMethodId);
        }

        [TestMethod]
        public void TestValidConfidenceIntervalMethod()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(LabelParameters.ParameterConfidenceIntervalMethod, "3");
            nameValues.Add(LabelParameters.ParameterAjaxKey, "1");
            LabelParameters parameters = new LabelParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.IsConfidenceIntervalMethodIdValid);
            Assert.AreEqual(3, parameters.ConfidenceIntervalMethodId);
        }

        [TestMethod]
        public void TestInvalid()
        {
            string[] parameterNames = new[] {ParameterNames.AgeId,
            ParameterNames.YearTypeId, LabelParameters.ParameterComparatorMethod};
            foreach (var parameter in parameterNames)
            {
                AssertInvalid(parameter, "");
                AssertInvalid(parameter, "-1");
                AssertInvalid(parameter, "0");
                AssertInvalid(parameter, null);
                AssertInvalid(parameter, "a");
            }

            LabelParameters parameters = new LabelParameters(new NameValueCollection());
            Assert.IsFalse(parameters.AreValid);
        }

        private static void AssertInvalid(string parameter, string s)
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(parameter, s);
            LabelParameters parameters = new LabelParameters(nameValues);
            Assert.IsFalse(parameters.AreValid);
        }
    }
}
