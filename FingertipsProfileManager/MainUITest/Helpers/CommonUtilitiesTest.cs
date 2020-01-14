using System;
using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class CommonUtilitiesTest
    {
        private const string urlKey = UrlKeys.HealthProfiles;

        [TestMethod]
        public void TestGetTestSiteUrl()
        {
            IndicatorGridModel model = new IndicatorGridModel
            {
                ProfileKey = urlKey,
                SelectedGroupId = 1,
                SelectedAreaTypeId = 2
            };

            string url = CommonUtilities.GetTestSiteUrl(model);
            Assert.IsTrue(url.Contains("ati/2"));
            Assert.IsTrue(url.Contains("gid/1"));
            Assert.IsTrue(url.Contains(urlKey + "#"));
        }

        [TestMethod]
        public void TestGetMonths()
        {
            Assert.AreEqual(12, CommonUtilities.GetMonths().Count());
        }

        [TestMethod]
        public void TestGetQuarters()
        {
            Assert.AreEqual(4, CommonUtilities.GetQuarters().Count());
        }

        [TestMethod]
        public void TestGetPolarities_PleaseSelectOptionIncludedAsRequested()
        {
            Assert.AreEqual(
                CommonUtilities.GetListOfPolarityTypes(PleaseSelectOption.NotRequired, ComparatorMethodIds.NoComparison).Count() +1,
                CommonUtilities.GetListOfPolarityTypes(PleaseSelectOption.Required, ComparatorMethodIds.NoComparison).Count()
                );
        }

        [TestMethod]
        public void GetListOfComparatorMethods_PleaseSelectOptionIncludedAsRequested()
        {
            Assert.AreEqual(
                CommonUtilities.GetListOfComparatorMethods(PleaseSelectOption.NotRequired).Count() +1,
                CommonUtilities.GetListOfComparatorMethods(PleaseSelectOption.Required).Count()
                );
        }

        [TestMethod]
        public void TestGetOrderedListOfDomainsWithGroupId_When_Valid_Profile()
        {
            var selectListItem = new SelectListItem {Value = UrlKeys.Phof};
            var domainList = CommonUtilities.GetOrderedListOfDomainsWithGroupId(new ProfileMembers(), selectListItem, new ProfileRepository(NHibernateSessionFactory.GetSession()));

            // Assert: expected number of domains
            Assert.AreEqual(6, domainList.Count());
        }

        [TestMethod]
        public void TestGetOrderedListOfDomainsWithGroupId_When_Profile_Is_Null()
        {
            var domainList = CommonUtilities.GetOrderedListOfDomainsWithGroupId(new ProfileMembers(), null, new ProfileRepository(NHibernateSessionFactory.GetSession()));

            // Assert
            Assert.AreEqual(0, domainList.Count());
        }
    }
}
