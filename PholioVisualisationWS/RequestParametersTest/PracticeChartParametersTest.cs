using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class PracticeChartParametersTest
    {
        [TestMethod]
        public void TestCacheKeyDifferentForEachParameterProperty()
        {
            AssertDifferentStringParameter(ParameterNames.AreaCode);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterAgeId1);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterAgeId2);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterDataPointOffset);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterGroupId1);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterGroupId2);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterHeight);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterIndicatorId1);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterIndicatorId2);
            AssertDifferentStringParameter(ParameterNames.ParentAreaCode);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterWidth);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterSexId1);
            AssertDifferentIntParameter(PracticeChartParameters.ParameterSexId2);
        }

        private static void AssertDifferentIntParameter(string name)
        {
            var parameters = Parameters();
            parameters[name] = 99.ToString();
            AssertAreDifferent(parameters);
        }

        private static void AssertDifferentStringParameter(string name)
        {
            var parameters = Parameters();
            parameters[name] = "z";
            AssertAreDifferent(parameters);
        }

        private static void AssertAreDifferent(NameValueCollection parameters)
        {
            Assert.AreNotEqual(
                new PracticeChartParameters(Parameters()).GetCacheKey(),
                new PracticeChartParameters(parameters).GetCacheKey()
                );
        }

        private static NameValueCollection Parameters()
        {
            NameValueCollection names = new NameValueCollection();
            names.Add(ParameterNames.AreaCode, "a");
            names.Add(PracticeChartParameters.ParameterAgeId1, 0.ToString());
            names.Add(PracticeChartParameters.ParameterAgeId2, 1.ToString());
            names.Add(PracticeChartParameters.ParameterDataPointOffset, 10.ToString());
            names.Add(PracticeChartParameters.ParameterGroupId1, 2.ToString());
            names.Add(PracticeChartParameters.ParameterGroupId2, 3.ToString());
            names.Add(PracticeChartParameters.ParameterHeight, 4.ToString());
            names.Add(PracticeChartParameters.ParameterIndicatorId1, 5.ToString());
            names.Add(PracticeChartParameters.ParameterIndicatorId2, 6.ToString());
            names.Add(ParameterNames.ParentAreaCode, "b");
            names.Add(PracticeChartParameters.ParameterWidth, 7.ToString());
            names.Add(PracticeChartParameters.ParameterSexId1, 8.ToString());
            names.Add(PracticeChartParameters.ParameterSexId2, 9.ToString());
            return names;
        }
    }
}
