
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class GroupDataAtDataPointParametersTest
    {
        [TestMethod]
        public void TestParse()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.GroupIds, "1");
            nameValues.Add(ParameterNames.ParentAreaCode, AreaCodes.Sha_EastOfEngland);
            nameValues.Add(ParameterNames.AreaTypeId, "5");

            GroupDataAtDataPointParameters parameters = new GroupDataAtDataPointParameters(nameValues);
            Assert.AreEqual(parameters.GroupId, 1);
        }

        [TestMethod]
        public void TestValidParameters()
        {
            Assert.IsTrue(new GroupDataAtDataPointParameters(GetValidParameters()).AreValid);
        }

        [TestMethod]
        public void TestInvalidParameters()
        {
            string[] names = new[]
            {
                ParameterNames.GroupIds
            };

            foreach (string name in names)
            {
                NameValueCollection nameValues = GetValidParameters();
                nameValues.Remove(name);
                Assert.IsFalse(new GroupDataAtDataPointParameters(nameValues).AreValid);
            }
        }

        private static NameValueCollection GetValidParameters()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.GroupIds, "1");
            nameValues.Add(ParameterNames.ParentAreaCode, AreaCodes.Sha_EastOfEngland);
            nameValues.Add(ParameterNames.ProfileId, "1");
            nameValues.Add(ParameterNames.AreaTypeId, "5");
            nameValues.Add("jsonp_callback", "a");
            return nameValues;
        }

    }
}
