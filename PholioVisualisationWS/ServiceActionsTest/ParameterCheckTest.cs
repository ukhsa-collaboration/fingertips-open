using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class ParameterCheckTest
    {
        public const string NameProfileId = "ProfileIdSupportingIndicators";

        [TestMethod]
        public void TestValidAreaCode()
        {
            ParameterCheck.ValidAreaCode(AreaCodes.CountyUa_Manchester);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestValidAreaCode_EmptyStringIsInvalid()
        {
            ParameterCheck.ValidAreaCode("");
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestValidAreaCode_NullIsInvalid()
        {
            ParameterCheck.ValidAreaCode(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestValidAreaCode_WhiteSpaceIsInvalid()
        {
            ParameterCheck.ValidAreaCode(" ");
        }

        [TestMethod]
        public void TestGreaterThanZero()
        {
            ParameterCheck.GreaterThanZero(NameProfileId, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestGreaterThanZero_ZeroInvalid()
        {
            ParameterCheck.GreaterThanZero(NameProfileId, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestGreaterThanZero_NegativeNumberInvalid()
        {
            ParameterCheck.GreaterThanZero(NameProfileId ,- 1);
        }
    }
}
