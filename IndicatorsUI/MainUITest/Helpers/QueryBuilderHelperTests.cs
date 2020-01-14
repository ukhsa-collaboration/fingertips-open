using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class QueryBuilderHelperTests
    {
        [TestMethod]
        public void Test_Should_Build_Where_Condition()
        {
            const string expectedWhereCondition = "WHERE testIdName1='testValueName1' OR testIdName1='testValueName2' OR testIdName2=7 OR testIdName2=8 OR testIdName3=1 OR testIdName3=0";

            var dictionary = new Dictionary<string, IEnumerable<object>>
            {
                { "testIdName1", new []{"testValueName1", "testValueName2" } } ,
                { "testIdName2", new object[] { 7, 8 } },
                { "testIdName3", new object[] { true, false } }
            };

            var result = QueryBuilderHelper.BuildWhereCondition(dictionary);

            Assert.AreEqual(expectedWhereCondition, result);
        }

        [TestMethod]
        public void Test_Should_Build_Value_Condition()
        {
            const string expectedWhereCondition = "VALUES('testValueName1',8,1,0)";

            var enumerable = new object[] {"testValueName1", 8, true, false};

            var result = QueryBuilderHelper.BuildValueCondition(enumerable);

            Assert.AreEqual(expectedWhereCondition, result);
        }
    }
}
