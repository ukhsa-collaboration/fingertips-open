using System;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Controllers;
using IndicatorsUI.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IndicatorsUI.MainUITest.Controllers
{
    [TestClass]
    public class AreaListControllerTest
    {
        private const string PublicId = "dI8niPkZM0";

        private Mock<IIdentityWrapper> _identityWrapper;
        private Mock<IAppConfig> _appConfig;
        private AreaListController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _identityWrapper = new Mock<IIdentityWrapper>();
            _appConfig = new Mock<IAppConfig>();
            _controller = new AreaListController(_identityWrapper.Object, _appConfig.Object);
        }

        [TestMethod]
        public void Test_Get_Area_List_Index_Page()
        {
            var result = (RedirectToRouteResult)_controller.Index();
            Assert.AreEqual("", result.RouteName);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Get_Area_List_Create_Page()
        {
            var result = (RedirectToRouteResult)_controller.Create();
            Assert.AreEqual("", result.RouteName);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Get_Area_List_Edit_Page()
        {
            var result = (RedirectToRouteResult)_controller.Edit(PublicId);
            Assert.AreEqual("", result.RouteName);
            VerifyAll();
        }

        private void VerifyAll()
        {
            _identityWrapper.VerifyAll();
        }
    }
}
