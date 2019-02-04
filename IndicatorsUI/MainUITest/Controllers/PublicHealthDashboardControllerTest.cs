using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Controllers;
using IndicatorsUI.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;


namespace IndicatorsUI.MainUITest.Controllers
{
    [TestClass]
    public class PublicHealthDashboardControllerTest
    {
        private PublicHealthDashboardControllerHelper _publicHealthDashboardController;

        private const string ValidProfileKey = "public-health-dashboard";
        private const string WrongProfileKey = "wrongProfileForTest";

        private const string HomeViewName = "Diabetes/DiabetesHome";
        private const string ErrorViewName = "../LongerLives/Error";
        private const string AboutDataViewName = "AboutData";
        private const string ConnectViewName = "Diabetes/DiabetesConnect";
        private const string PracticeRankingViewName = "Diabetes/DiabetesRankings";
        private const string PracticeDetailsViewName = "Diabetes/DiabetesPracticeDetails";
        private const string AreaDetailsViewName = "Diabetes/DiabetesAreaDetails";

        private enum PolicyType { Privacy, Accessibility, Cookies };

        private const string PrivacyViewName = "PolicyPrivacy";
        private const string AccessibilityViewName = "PolicyAccessibility";
        private const string CookiesViewName = "PolicyCookies";

        [TestInitialize]
        public void Init()
        {
            var request = new Mock<HttpRequestBase>();

            var profileReader = ReaderFactory.GetProfileReader();
            var appConfig = new AppConfig(ConfigurationManager.AppSettings);

            request.SetupGet(x => x.Headers).Returns( new System.Net.WebHeaderCollection {
                { "X-Requested-With", "XMLHttpRequest"}
            });

            request.SetupGet(x => x.Url).Returns(new Uri("http://localhost:59822"));

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            _publicHealthDashboardController = new PublicHealthDashboardControllerHelper(profileReader, appConfig);
            _publicHealthDashboardController.ControllerContext = new ControllerContext(context.Object, new RouteData(), _publicHealthDashboardController);

        }

        [TestMethod]
        public void Test_Should_Call_Home()
        {
            // Act
            var actionResult = _publicHealthDashboardController.Home() as ViewResult;

            // Assert
            AssertExpectedViewResult(HomeViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_Alternavite_Home()
        {
            // Act
            var actionResult = _publicHealthDashboardController.Home(ValidProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(HomeViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Response_Error_view_Alternavite_Home()
        {
            // Act
            var actionResult = _publicHealthDashboardController.Home(WrongProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(ErrorViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_MapWithData()
        {
            // Act
            var actionResult = _publicHealthDashboardController.MapWithData(ValidProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(HomeViewName, actionResult);
            Assert.IsTrue(actionResult != null && actionResult.ViewBag.MapNoData);
        }

        [TestMethod]
        public void Test_Should_Response_Error_view_MapWithData()
        {
            // Act
            var actionResult = _publicHealthDashboardController.MapWithData(WrongProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(ErrorViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_AboutData()
        {
            // Act
            var actionResult = _publicHealthDashboardController.AboutData(ValidProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(AboutDataViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Response_Error_view_AboutData()
        {
            // Act
            var actionResult = _publicHealthDashboardController.AboutData(WrongProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(ErrorViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_Alternative_AboutData()
        {
            // Act
            var actionResult = _publicHealthDashboardController.SendToAboutData() as ViewResult;

            // Assert
            AssertExpectedViewResult(AboutDataViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_PracticeRankings()
        {
            // Act
            var actionResult = _publicHealthDashboardController.PracticeRankings(ValidProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(PracticeRankingViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Response_Error_view_PracticeRankings()
        {
            // Act
            var actionResult = _publicHealthDashboardController.PracticeRankings(WrongProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(ErrorViewName, actionResult);
        }


        [TestMethod]
        public void Test_Should_Call_AreaDetails()
        {
            // Act
            var actionResult = _publicHealthDashboardController.AreaDetails(ValidProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(AreaDetailsViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_Policy_Privacy()
        {
            // Act
            var actionResult = _publicHealthDashboardController.Policy(PolicyType.Privacy.ToString()) as ViewResult;

            // Assert
            AssertExpectedViewResult(PrivacyViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_Policy_Accessibility()
        {
            // Act
            var actionResult = _publicHealthDashboardController.Policy(PolicyType.Accessibility.ToString()) as ViewResult;

            // Assert
            AssertExpectedViewResult(AccessibilityViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Call_Policy_Cookies()
        {
            // Act
            var actionResult = _publicHealthDashboardController.Policy(PolicyType.Cookies.ToString()) as ViewResult;

            // Assert
            AssertExpectedViewResult(CookiesViewName, actionResult);
        }

        [TestMethod]
        public void Test_Should_Response_Error_For_Not_Expected_Policy()
        {
            // Act
            var actionResult = _publicHealthDashboardController.Policy(WrongProfileKey) as ViewResult;

            // Assert
            AssertExpectedViewResult(ErrorViewName, actionResult);
        }

        private void AssertExpectedViewResult(string expectedViewName, ViewResult actionResult)
        {
            Assert.AreEqual(expectedViewName, actionResult.ViewName);
        }
    }

    internal class PublicHealthDashboardControllerHelper : LongerLivesController
    {
        public PublicHealthDashboardControllerHelper(ProfileReader profileReader, IAppConfig appConfig) : base(profileReader, appConfig)
        {
            InitializePage();
        }

        private void InitializePage()
        {
            if (PageModel == null)
            {
                ProfileCollectionBuilder = new ProfileCollectionBuilder(ReaderFactory.GetProfileReader(), _appConfig);
                NewPageModel();

                PageModel.ProfileCollections = new List<ProfileCollection>();

                PageModel.NationalProfileCollection =
                    ProfileCollectionBuilder.GetCollection(ProfileCollectionIds.NationalProfiles);

                PageModel.HighlightedProfileCollection =
                    ProfileCollectionBuilder.GetCollection((ProfileCollectionIds.HighlightedProfiles));
            }
            ViewBag.FeatureSwitchJavaScript = JsonConvert.SerializeObject(_appConfig.ActiveFeatures);
            ViewBag.IsUserSignedIn = UserAccountHelper.IsUserSignedIn(User);
        }
    }
}