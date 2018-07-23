using System;
using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class AreaSearchHelperTest
    {

        [TestMethod]
        public void Test_Search_Not_Available_For_CCGs_Post_Apr2017()
        {
            var isSearch = new AreaSearchHelper(ProfileIds.Undefined,
                AreaTypeIds.CcgPostApr2017.ToString()).IsPlaceNameSearchAvailable;
            Assert.IsFalse(isSearch);
        }

        [TestMethod]
        public void Test_Search_Available_For_CCGs_Pre_Apr2017()
        {
            var isSearch = new AreaSearchHelper(ProfileIds.Undefined,
                AreaTypeIds.CcgPreApr2017.ToString()).IsPlaceNameSearchAvailable;
            Assert.IsTrue(isSearch);
        }

        [TestMethod]
        public void Test_GetAreaTypeLabel_CCG()
        {
            var label = new AreaSearchHelper(ProfileIds.Undefined,
                AreaTypeIds.CcgPostApr2017.ToString()).GetAreaTypeLabelSingular();
            Assert.AreEqual("CCG", label);
        }

        [TestMethod]
        public void Test_GetSearchHeading_HealthProfile()
        {
            var heading = new AreaSearchHelper(ProfileIds.HealthProfiles, null).GetSearchHeading();
            Assert.AreEqual("Find your Health Profile", heading);
        }
    }
}
