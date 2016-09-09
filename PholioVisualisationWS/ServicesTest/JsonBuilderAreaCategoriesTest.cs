using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class JsonBuilderAreaCategoriesTest
    {
        private static string json;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var parameters = new NameValueCollection();
            parameters.Add(ParameterNames.AreaTypeId, AreaTypeIds.CountyAndUnitaryAuthority.ToString());
            parameters.Add(ParameterNames.CategoryTypeId, CategoryTypeIds.DeprivationDecileCountyAndUA2010.ToString());
            parameters.Add(ParameterNames.ProfileIdFull, ProfileIds.LongerLives.ToString());

            var request = new Mock<HttpContextBase>();
            request.Setup(x => x.Request.Params).Returns(parameters);

            json = new JsonBuilderAreaCategories(request.Object).GetJson();
        }

        [TestMethod]
        public void TestAreasAreaIgnoredBasedOnProfileId()
        {
            Assert.IsFalse(json.Contains(AreaCodes.CountyUa_IslesOfScilly), "Isles of Scilly should have been ignored");
        }

        [TestMethod]
        public void TestExpectedNumberOfAreas()
        {
            Assert.AreEqual(150, json.Count(x => x == ':'));
        }


    }
}
