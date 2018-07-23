using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IndicatorsUI.MainUI.Controllers;
using IndicatorsUI.UserAccess;
using IndicatorsUI.UserAccess.UserList.IRepository;
using IndicatorsUI.MainUI;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models.UserAccess;
using IndicatorsUI.MainUI.Models.UserList;

namespace IndicatorsUI.MainUITest.Controllers
{
    [TestClass]
    public class IndicatorListControllerTest
    {
        public const string PublicListId = "abc";
        public const int ListId = 2;
        private static readonly string _userId = Guid.NewGuid().ToString();

        private Mock<IIndicatorListRepository> _indicatorRepo;
        private Mock<IIdentityWrapper> _identityWrapper;
        private Mock<IExceptionLoggerWrapper> _exceptionLoggerWrapper;
        private Mock<IPublicIdGenerator> _publicIdGenerator;
        private Mock<IAppConfig> _appConfig;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            AutoMapperConfig.RegisterMappings();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _indicatorRepo = new Mock<IIndicatorListRepository>(MockBehavior.Strict);
            _identityWrapper = new Mock<IIdentityWrapper>(MockBehavior.Strict);
            _exceptionLoggerWrapper = new Mock<IExceptionLoggerWrapper>(MockBehavior.Strict);
            _publicIdGenerator = new Mock<IPublicIdGenerator>(MockBehavior.Strict);
            _appConfig = new Mock<IAppConfig>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Index_ExpectViewResultReturned()
        {
            SetUpUserIsSignedIn();
            SetUpGetApplicationUser();
            _indicatorRepo.Setup(s => s.GetAll(It.IsAny<string>())).Returns(GetIndicatorLists());

            var controller = GetIndicatorListController();
            controller.ControllerContext = GetControllerContextForMockUser();

            var result = (ViewResult)controller.Index();
            Assert.AreEqual("IndicatorListIndex", result.ViewName);
            VerifyAll();
        }

        [TestMethod]
        public void Index_Cannot_Be_Reached_If_Signed_Out()
        {
            SetUpUserIsNotSignedIn();
            var result = (RedirectToRouteResult)GetIndicatorListController().Index();
            AssertNotAuthenticatedRedirect(result);
            VerifyAll();
        }

        [TestMethod]
        public void Create_ExpectViewResultReturned()
        {
            SetUpUserIsSignedIn();
            var controller = GetIndicatorListController();
            var result = (ViewResult)controller.Create();
            Assert.AreEqual("CreateIndicatorList", result.ViewName);
            VerifyAll();
        }

        [TestMethod]
        public void Create_Cannot_Be_Reached_If_Signed_Out()
        {
            SetUpUserIsNotSignedIn();
            var result = (RedirectToRouteResult)GetIndicatorListController().Create();
            AssertNotAuthenticatedRedirect(result);
            VerifyAll();
        }

        [TestMethod]
        public void Edit_Cannot_Be_Reached_If_Signed_Out()
        {
            SetUpUserIsNotSignedIn();
            var result = (RedirectToRouteResult)GetIndicatorListController().Edit(null);
            AssertNotAuthenticatedRedirect(result);
            VerifyAll();
        }

        [TestMethod]
        public void Delete_When_User_Owns_List()
        {
            SetUpUserIsSignedIn();
            SetUpGetApplicationUser();

            _indicatorRepo.Setup(x => x.DoesUserOwnList(PublicListId, _userId))
                .Returns(true);
            _indicatorRepo.Setup(x => x.Delete(ListId));
            SetUpGetListByPublicId();

            // Act: Delete list
            var result = (RedirectToRouteResult)GetIndicatorListController()
                .Delete(PublicListId);

            // Assert
            ControllerTestHelper.AssertRedirectAction(result, "Index", "IndicatorList");
            VerifyAll();
        }

        [TestMethod]
        public void Delete_When_User_Does_Not_Own_List()
        {
            SetUpUserIsSignedIn();
            SetUpGetApplicationUser();

            _indicatorRepo.Setup(x => x.DoesUserOwnList(PublicListId, _userId))
                .Returns(false);

            // Act: Delete list (will not call delete method)
            var result = (RedirectToRouteResult)GetIndicatorListController()
                .Delete(PublicListId);

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"].ToString());
            Assert.AreEqual("IndicatorList", result.RouteValues["controller"].ToString());
            VerifyAll();
        }

        [TestMethod]
        public void Delete_Cannot_Be_Reached_If_Signed_Out()
        {
            SetUpUserIsNotSignedIn();
            var result = (RedirectToRouteResult)GetIndicatorListController().Delete(null);
            AssertNotAuthenticatedRedirect(result);
            VerifyAll();
        }

        [TestMethod]
        public void Save_New_List()
        {
            SetUpUserIsSignedIn();
            SetUpGetApplicationUser();

            _publicIdGenerator.Setup(x => x.GetIndicatorListPublicId())
                .Returns(PublicListId);

            IndicatorListViewModel indicator =
                new IndicatorListViewModel()
                {
                    Id = 0,
                    UserId = _userId,
                    IndicatorListItems = null,
                    ListName = "TestList"
                };
            _indicatorRepo.Setup(x => x.Save(It.IsAny<IndicatorList>()))
                .Returns(ListId);

            var controller = GetIndicatorListController();
            controller.ControllerContext = GetControllerContextForMockUser();

            var result = (JsonResult)controller.Save(indicator);

            // Assert
            AssertResultIsSuccessfulSave(result);

            VerifyAll();
        }

        [TestMethod]
        public void Save_Existing_List_Where_User_Owns_List()
        {
            SetUpUserIsSignedIn();
            SetUpGetApplicationUser();

            _indicatorRepo.Setup(x => x.DoesUserOwnList(PublicListId, _userId))
                .Returns(true);

            IndicatorListViewModel indicatorListVm =
                new IndicatorListViewModel()
                {
                    Id = ListId,
                    PublicId = PublicListId
                };
            _indicatorRepo.Setup(x => x.Update(It.IsAny<IndicatorList>()));

            var controller = GetIndicatorListController();
            controller.ControllerContext = GetControllerContextForMockUser();

            var result = (JsonResult)controller.Save(indicatorListVm);

            // Assert
            AssertResultIsSuccessfulSave(result);

            VerifyAll();
        }

        [TestMethod]
        public void Save_Existing_List_Where_User_Does_Not_Own_List()
        {
            SetUpUserIsSignedIn();
            SetUpGetApplicationUser();

            _indicatorRepo.Setup(x => x.DoesUserOwnList(PublicListId, _userId))
                .Returns(false);

            IndicatorListViewModel indicatorListVm =
                new IndicatorListViewModel()
                {
                    Id = ListId,
                    PublicId = PublicListId
                };

            var controller = GetIndicatorListController();
            controller.ControllerContext = GetControllerContextForMockUser();

            var result = (JsonResult)controller.Save(indicatorListVm);

            // Assert
            AssertResultIsNotSuccess(result, "You can only save lists that you have created");

            VerifyAll();
        }

        [TestMethod]
        public void Save_Cannot_Be_Reached_If_Signed_Out()
        {
            SetUpUserIsNotSignedIn();
            var result = (RedirectToRouteResult)GetIndicatorListController().Save(null);
            AssertNotAuthenticatedRedirect(result);
            VerifyAll();
        }

        [TestMethod]
        public void GetTopIndicatorList_ReturnCorrectPartialView()
        {
            SetUpGetApplicationUser();

            _indicatorRepo.Setup(x => x.GetTopIndicatorList(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(GetIndicatorLists());

            var controller = GetIndicatorListController();
            controller.ControllerContext = GetControllerContextForMockUser();

            var result = (PartialViewResult)controller.GetTopIndicatorList();

            // Assert
            Assert.AreEqual("_IndicatorListCollectionLinks", result.ViewName);
            Assert.IsInstanceOfType(result.Model, typeof(IndicatorListsViewModel));

            VerifyAll();
        }

        private static void AssertResultIsNotSuccess(JsonResult result, string message)
        {
            var taskResult = (TaskResult)result.Data;
            Assert.AreEqual(message, taskResult.Message);
            Assert.IsFalse(taskResult.Success);
        }

        private static void AssertResultIsSuccessfulSave(JsonResult result)
        {
            var taskResult = (TaskResult)result.Data;
            Assert.AreEqual("Saved successfully", taskResult.Message);
            Assert.IsTrue(taskResult.Success);
        }

        private static void AssertNotAuthenticatedRedirect(RedirectToRouteResult result)
        {
            ControllerTestHelper.AssertRedirectAction(result, "Login", "UserAccount");
        }

        private static IEnumerable<IndicatorList> GetIndicatorLists()
        {
            ICollection<IndicatorListItem> listItems1 = new List<IndicatorListItem>()
            {
                new IndicatorListItem() {Id = 1, AgeId = 1, SexId = 1, IndicatorId = 1, ListId = 1},
                new IndicatorListItem() {Id = 2, AgeId = 2, SexId = 2, IndicatorId = 2, ListId = 1},
            };
            ICollection<IndicatorListItem> listItems2 = new List<IndicatorListItem>()
            {
                new IndicatorListItem() {Id = 3, AgeId = 1, SexId = 1, IndicatorId = 1, ListId = 2},
                new IndicatorListItem() {Id = 4, AgeId = 2, SexId = 2, IndicatorId = 2, ListId = 2},
            };
            IEnumerable<IndicatorList> indicatorList = new List<IndicatorList>()
            {
                new IndicatorList() { Id = 1, ListName = "Test", IndicatorListItems = listItems1, UserId = "TestUser1",
                    CreatedOn = DateTime.Now},
                new IndicatorList(){ Id = 2, ListName = "Test", IndicatorListItems = listItems2, UserId = "TestUser2",
                    CreatedOn = DateTime.Now},
            };
            return indicatorList;
        }

        private void VerifyAll()
        {
            _exceptionLoggerWrapper.VerifyAll();
            _indicatorRepo.VerifyAll();
            _identityWrapper.VerifyAll();
            _publicIdGenerator.VerifyAll();
        }

        private static ControllerContext GetControllerContextForMockUser()
        {
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("Test@phe.gov.uk");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            return controllerContext.Object;
        }

        private void SetUpGetListByPublicId()
        {
            _indicatorRepo.Setup(x => x.GetListByPublicId(PublicListId))
                .Returns(GetIndicatorList());
        }

        private void SetUpGetApplicationUser()
        {
            _identityWrapper.Setup(s => s.GetApplicationUser(It.IsAny<IPrincipal>()))
                .Returns(GetApplicationUser);
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

        public IndicatorList GetIndicatorList()
        {
            return new IndicatorList
            {
                Id = ListId,
                PublicId = PublicListId
            };
        }

        public ApplicationUser GetApplicationUser()
        {
            return new ApplicationUser
            {
                Id = _userId,
                FirstName = "test",
                LastName = "tester"
            };
        }

        private IndicatorListController GetIndicatorListController()
        {
            var controller = new IndicatorListController(_indicatorRepo.Object,
                _identityWrapper.Object, _exceptionLoggerWrapper.Object,
                _publicIdGenerator.Object, _appConfig.Object
            );
            return controller;
        }
    }
}
