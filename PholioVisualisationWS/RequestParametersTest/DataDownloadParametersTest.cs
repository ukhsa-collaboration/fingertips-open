
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.RequestParametersTest
{
    [TestClass]
    public class DataDownloadParametersTest
    {
        [TestMethod]
        public void TestValid()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.ProfileId, "1");
            nameValues.Add(ParameterNames.ParentAreaCode, AreaCodes.Sha_EastOfEngland);
            nameValues.Add(ParameterNames.AreaTypeId, (AreaTypeIds.Pct).ToString());
            nameValues.Add(ParameterNames.ParentsToDisplay, ((int)ParentDisplay.NationalOnly).ToString());

            DataDownloadParameters parameters = new DataDownloadParameters(nameValues);
            Assert.AreEqual(1, parameters.ProfileId);
            Assert.AreEqual(AreaCodes.Sha_EastOfEngland, parameters.ParentAreaCode);
            Assert.AreEqual(AreaTypeIds.Pct, parameters.AreaTypeId);
            Assert.AreEqual(ParentDisplay.NationalOnly, parameters.ParentsToDisplay);

            Assert.IsFalse(parameters.UseIndicatorIds);
            Assert.IsTrue(parameters.AreValid);
        }

        [TestMethod]
        public void TestUseIndicatorIds()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.ProfileId, "2");
            nameValues.Add(DataDownloadParameters.ParameterIndicatorIds, "1,2");
            nameValues.Add(ParameterNames.ParentAreaCode, AreaCodes.Sha_EastOfEngland);
            nameValues.Add(ParameterNames.AreaTypeId, (AreaTypeIds.Pct).ToString());
            nameValues.Add(ParameterNames.ParentsToDisplay, ((int)ParentDisplay.NationalOnly).ToString());

            DataDownloadParameters parameters = new DataDownloadParameters(nameValues);
            Assert.IsTrue(parameters.AreValid);
            Assert.IsTrue(parameters.UseIndicatorIds);
        }

        [TestMethod]
        public void TestParseParentsToDisplay()
        {
            // Valid
            Assert.IsTrue(DataDownloadParameters.IsParentDisplayValid((int)ParentDisplay.NationalAndRegional));
            Assert.IsTrue(DataDownloadParameters.IsParentDisplayValid((int)ParentDisplay.RegionalOnly));
            Assert.IsTrue(DataDownloadParameters.IsParentDisplayValid((int)ParentDisplay.NationalOnly));

            // Invalid
            Assert.IsFalse(DataDownloadParameters.IsParentDisplayValid((int)ParentDisplay.Undefined));
            Assert.IsFalse(DataDownloadParameters.IsParentDisplayValid(DataDownloadParameters.UndefinedInteger));
        }
    }
}
