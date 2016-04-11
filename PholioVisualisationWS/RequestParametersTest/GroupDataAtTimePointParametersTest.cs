
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class GroupDataAtTimePointParametersTest
    {

        [TestMethod]
        public void ValueIsParsedCorrectly()
        {
            NameValueCollection nameValues = GetValidParameters();
            nameValues[GroupDataAtTimePointParameters.ParameterYear] = "2001";
            nameValues[GroupDataAtTimePointParameters.ParameterYearRange] = "2";
            GroupDataAtTimePointParameters parameters = new GroupDataAtTimePointParameters(nameValues);
            Assert.AreEqual(2001, parameters.TimePeriod.Year);
            Assert.AreEqual(2, parameters.TimePeriod.YearRange);
            Assert.AreEqual(-1, parameters.TimePeriod.Quarter);
            Assert.AreEqual(-1, parameters.TimePeriod.Month);
        }

        [TestMethod]
        public void TestInvalidParameters()
        {
            string[] names = new[]
            {
                GroupDataAtTimePointParameters.ParameterYear,
                GroupDataAtTimePointParameters.ParameterYearRange
            };

            foreach (string name in names)
            {
                NameValueCollection nameValues = GetValidParameters();
                nameValues.Remove(name);
                Assert.IsFalse(new GroupDataAtTimePointParameters(nameValues).AreValid);
            }
        }

        [TestMethod]
        public void AreValid()
        {
            Assert.IsTrue(new GroupDataAtTimePointParameters(GetValidParameters()).AreValid);
        }

        [TestMethod]
        public void InvalidYears()
        {
            // Too low
            NameValueCollection nameValues = GetValidParameters();
            nameValues[GroupDataAtTimePointParameters.ParameterYear] = "-2";
            GroupDataAtTimePointParameters parameters = new GroupDataAtTimePointParameters(nameValues);
            Assert.IsFalse(parameters.AreValid);

            // Too high
            nameValues = GetValidParameters();
            nameValues[GroupDataAtTimePointParameters.ParameterYear] = "10000";
            parameters = new GroupDataAtTimePointParameters(nameValues);
            Assert.IsFalse(parameters.AreValid);

            // Not a number
            nameValues = GetValidParameters();
            nameValues[GroupDataAtTimePointParameters.ParameterYear] = "a";
            parameters = new GroupDataAtTimePointParameters(nameValues);
            Assert.IsFalse(parameters.AreValid);
        }

        [TestMethod]
        public void InvalidYearRanges()
        {
            // Too low
            NameValueCollection nameValues = GetValidParameters();
            nameValues[GroupDataAtTimePointParameters.ParameterYearRange] = "-2";
            GroupDataAtTimePointParameters parameters = new GroupDataAtTimePointParameters(nameValues);
            Assert.IsFalse(parameters.AreValid);

            // Too high
            nameValues = GetValidParameters();
            nameValues[GroupDataAtTimePointParameters.ParameterYearRange] = "10000";
            parameters = new GroupDataAtTimePointParameters(nameValues);
            Assert.IsFalse(parameters.AreValid);

            // Not a number
            nameValues = GetValidParameters();
            nameValues[GroupDataAtTimePointParameters.ParameterYearRange] = "a";
            parameters = new GroupDataAtTimePointParameters(nameValues);
            Assert.IsFalse(parameters.AreValid);
        }

        private static NameValueCollection GetValidParameters()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.GroupIds, "1");
            nameValues.Add(ParameterNames.AreaTypeId, "2");
            nameValues.Add(GroupDataAtTimePointParameters.ParameterYear, "2005");
            nameValues.Add(GroupDataAtTimePointParameters.ParameterYearRange, "1");
            nameValues.Add(ParameterNames.ParentAreaCode, AreaCodes.Sha_EastOfEngland);
            nameValues.Add(ParameterNames.ProfileId, "1");
            nameValues.Add("jsonp_callback", "a");
            return nameValues;
        }
    }
}
