
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class GroupDataAtDataPointBySearchParametersTest
    {

        [TestMethod]
        public void TestValidParameters()
        {
            NameValueCollection nameValues = GetValidParameters();
            GroupDataAtDataPointBySearchParameters parameters = new GroupDataAtDataPointBySearchParameters(nameValues);
            Assert.AreEqual(AreaCodes.Sha_EastOfEngland, parameters.ParentAreaCode);
            Assert.IsTrue(parameters.IndicatorIds.Contains(2) && parameters.IndicatorIds.Contains(3));

            Assert.IsTrue(parameters.AreValid);
        }

        [TestMethod]
        public void TestZeroIndicatorIdsIsValid()
        {
            NameValueCollection nameValues = GetValidParameters();
            GroupDataAtDataPointBySearchParameters parameters = new GroupDataAtDataPointBySearchParameters(nameValues);
            nameValues[GroupDataAtDataPointBySearchParameters.ParameterIndicatorIds] = "";
            Assert.IsTrue(parameters.AreValid);
        }

        private NameValueCollection GetValidParameters()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(GroupDataAtDataPointBySearchParameters.ParameterIndicatorIds, "2,3");
            nameValues.Add(ParameterNames.ParentAreaCode, AreaCodes.Sha_EastOfEngland);
            nameValues.Add(ParameterNames.AreaTypeId, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019.ToString());
            return nameValues;
        }

        [TestMethod]
        public void TestRestrictedProfileId()
        {
            // Both restricted and profile id set
            NameValueCollection nameValues = GetValidParameters();
            nameValues.Add(ParameterNames.ProfileId, "1");
            nameValues.Add(ParameterNames.RestrictToProfileId, "1,2");
            GroupDataAtDataPointBySearchParameters parameters = new GroupDataAtDataPointBySearchParameters(nameValues);
            Assert.AreEqual(2, parameters.RestrictResultsToProfileIdList.Count);
            Assert.AreEqual(1, parameters.ProfileId);
        }
    }

}
