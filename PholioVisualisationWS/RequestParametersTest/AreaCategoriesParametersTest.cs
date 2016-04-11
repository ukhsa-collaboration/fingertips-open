using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class AreaCategoriesParametersTest
    {
        [TestMethod]
        public void TestValidParameters()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.AreaTypeId, "1");
            nameValues.Add(ParameterNames.CategoryTypeId, "2");
            AreaCategoriesParameters parameters = new AreaCategoriesParameters(nameValues);

            Assert.AreEqual(1, parameters.ChildAreaTypeId);
            Assert.AreEqual(2, parameters.CategoryTypeId);
            Assert.IsTrue(parameters.AreValid);
        }

        [TestMethod]
        public void TestInvalidWithOnlyOneParameterSpecified()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.AreaTypeId, "1");
            Assert.IsFalse(new AreaCategoriesParameters(nameValues).AreValid);

            nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.ParentAreaTypeId, "1");
            Assert.IsFalse(new AreaCategoriesParameters(nameValues).AreValid);
        }
    }
}
