using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI;
using IndicatorsUI.MainUI.Controllers;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models.UserAccess;
using IndicatorsUI.UserAccess;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IndicatorsUI.MainUITest.Controllers
{
    [TestClass]
    public class UserAccountControllerTest
    {
        private const string EmailAddress = "b@a.com";
        private const string Password = "aaaaaaa1";

        private Mock<IIdentityWrapper> _identityWrapper;
        private Mock<IUserAccessDbContext> _userAccessDbContext;
        private Mock<IExceptionLoggerWrapper> _exceptionLogger;
        private Mock<IUserAccountHelper> _userAccountHelper;
        private Mock<IAppConfig> _appConfig;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            AutoMapperConfig.RegisterMappings();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _identityWrapper = new Mock<IIdentityWrapper>();
            _userAccessDbContext = new Mock<IUserAccessDbContext>();
            _exceptionLogger = new Mock<IExceptionLoggerWrapper>();
            _userAccountHelper = new Mock<IUserAccountHelper>();
            _appConfig = new Mock<IAppConfig>();
        }

        [TestMethod]
        public void Test_Get_Login_Page()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection {
                    {"X-Requested-With", "XMLHttpRequest"}
                });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = GetController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = (ViewResult) controller.Login();
            
            Assert.AreEqual("", result.ViewName);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Sign_In_Without_Registered_Email_Address()
        {
            var controller = GetController();
            var viewModel = new LoginViewModel
            {
                UserName = EmailAddress
            };

            var result = (ViewResult) controller.ValidateLogin(viewModel).Result;

            // Assert
            Assert.AreEqual("Login", result.ViewName);
            ControllerTestHelper.AssertModelStateIsNotValid(result);
            var errors = ControllerTestHelper.GetModelStateErrors(result);
            Assert.AreEqual(errors.Count(), 1);
            Assert.AreEqual(UserAccountController.ErrorEmailAddressNotRecognised, errors.First());
            VerifyAll();
        }

        [TestMethod]
        public void Test_Get_Registration_Page()
        {
            var controller = GetController();
            var result = (ViewResult) controller.Registration();
            Assert.AreEqual("", result.ViewName);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Request_Another_Email_From_Sign_In_Page_Success()
        {
            // Arrange: valid user returned
            SetUpGetValidApplicationUserFromUserAccessDbContext();

            var viewModel = new LoginViewModel {UserName = EmailAddress};

            // Act
            var result = (ViewResult) GetController().RequestAnotherEmail(viewModel);

            // Assert
            Assert.AreEqual("Thankyou", result.ViewName);
            VerifyAll();
        }

        [Ignore]
        [TestMethod]
        public void Test_Request_Another_Email_From_Sign_In_Page_Failure()
        {
            // Arrange: exception is logged
            _exceptionLogger.Setup(x => x.LogException(It.IsAny<Exception>(), It.IsAny<string>()));

            SetUpGetValidApplicationUserFromUserAccessDbContext();
            var viewModel = new LoginViewModel {UserName = EmailAddress};

            // Act
            var result = (RedirectToRouteResult) GetController().RequestAnotherEmail(viewModel);

            // Assert: redirected to error page
            Assert.IsTrue(result.RouteValues.ContainsValue("EmailError"));
            VerifyAll();
        }

        [TestMethod]
        public void Test_Submit_Registration_Success()
        {
            _appConfig.Setup(x => x.IsFeatureActive(FeatureFlags.EmailVerification))
                .Returns(true);

            // Arrange: user name found
            _userAccountHelper.Setup(x => x.GetUserName(It.IsAny<IPrincipal>(), EmailAddress))
                .Returns(EmailAddress);

            var viewModel = GetValidRegisterViewModel();

            SetUpGetValidApplicationUserFromUserAccessDbContext();

            // Arrange: user is created
            var task = Task.FromResult(IdentityResult.Success);
            _userAccountHelper.Setup(x => x.CreateUser(It.IsAny<HttpRequestBase>(), It.IsAny<ApplicationUser>(),
                Password)).Returns(task);

            // Act
            var result = (ViewResult) GetController().SubmitRegistration(viewModel).Result;

            // Assert
            Assert.AreEqual("Thankyou", result.ViewName);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Submit_Registration_Fails_With_Model_Error()
        {
            var viewModel = GetValidRegisterViewModel();

            var controller = GetController();
            controller.ModelState.AddModelError("UserName", "not valid");

            // Act
            var result = (ViewResult) controller.SubmitRegistration(viewModel).Result;

            // Assert: User gets the same page again
            Assert.AreEqual("Registration", result.ViewName);
            ControllerTestHelper.AssertModelStateIsNotValid(result);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Sign_Out()
        {
            _userAccountHelper.Setup(x => x.SignOut(It.IsAny<HttpRequestBase>()));

            // Act
            var result = (RedirectToRouteResult) GetController().Logout();

            // Assert
            Assert.IsTrue(result.RouteValues.ContainsValue("SignedOut"));
            VerifyAll();
        }

        [TestMethod]
        public void Test_Signed_Out()
        {
            // Act
            var result = (ViewResult) GetController().SignedOut();

            // Assert
            Assert.AreEqual("Message", result.ViewName);
            Assert.IsNotNull(result.Model as PageMessageViewModel);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Edit_My_Account()
        {
            SetUpUserIsSignedIn();

            _identityWrapper.Setup(x => x.GetApplicationUser(It.IsAny<IPrincipal>()))
                .Returns(GetValidApplicationUser);

            // Act
            var result = (ViewResult) GetController().EditMyAccount();

            // Assert
            Assert.AreEqual("EditMyAccount", result.ViewName);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Edit_My_Account_Cannot_Be_Accessed_If_Signed_Out()
        {
            SetUpUserIsNotSignedIn();
            var result = (RedirectToRouteResult)GetController().EditMyAccount();
            ControllerTestHelper.AssertRedirectAction(result, "Login");
            VerifyAll();
        }

        private void SetUpGetValidApplicationUserFromUserAccessDbContext()
        {
            _userAccessDbContext.Setup(x => x.GetUser(EmailAddress))
                .Returns(GetValidApplicationUser());
        }

        private static ApplicationUser GetValidApplicationUser()
        {
            return new ApplicationUser
            {
                UserName = EmailAddress,
                FirstName = "a",
                TempGuid = Guid.NewGuid()
            };
        }

        private static RegisterViewModel GetValidRegisterViewModel()
        {
            var viewModel = new RegisterViewModel
            {
                UserName = EmailAddress,
                Password = Password,
                ConfirmPassword = Password,
                FirstName = "a",
                LastName = "b"
            };
            return viewModel;
        }

        private UserAccountController GetController()
        {
            return new UserAccountController(_identityWrapper.Object, _userAccessDbContext.Object,
                _exceptionLogger.Object, _userAccountHelper.Object, _appConfig.Object);
        }

        private void SetUpUserIsSignedIn()
        {
            _identityWrapper.Setup(x => x.IsUserSignedIn(It.IsAny<IPrincipal>()))
                .Returns(true);
        }

        private void SetUpUserIsNotSignedIn()
        {
            _identityWrapper.Setup(x => x.IsUserSignedIn(It.IsAny<IPrincipal>()))
                .Returns(false);
        }

        private void VerifyAll()
        {
            _identityWrapper.VerifyAll();
            _userAccessDbContext.VerifyAll();
            _exceptionLogger.VerifyAll();
            _userAccountHelper.VerifyAll();
        }
    }
}