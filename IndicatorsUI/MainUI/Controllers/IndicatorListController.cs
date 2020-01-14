using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using IndicatorsUI.MainUI.Models.UserList;
using System.ComponentModel.DataAnnotations;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models.UserAccess;
using IndicatorsUI.UserAccess;
using IndicatorsUI.UserAccess.UserList.IRepository;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace IndicatorsUI.MainUI.Controllers
{
    [RoutePrefix("user-account/indicator-list")]
    public class IndicatorListController : BaseController
    {
        private readonly IIndicatorListRepository _indicatorListRepository;
        private readonly IIdentityWrapper _identity;
        private readonly IExceptionLoggerWrapper _exceptionLoggerWrapper;
        private IPublicIdGenerator _publicIdGenerator;

        public IndicatorListController(IIndicatorListRepository indicatorListRepository,
            IIdentityWrapper identity, IExceptionLoggerWrapper exceptionLoggerWrapper,
            IPublicIdGenerator publicIdGenerator, IAppConfig appConfig) : base(appConfig)

        {
            _indicatorListRepository = indicatorListRepository;
            _identity = identity;
            _exceptionLoggerWrapper = exceptionLoggerWrapper;
            _publicIdGenerator = publicIdGenerator;
        }

        [Route("")]
        public ActionResult Index()
        {

            // Check user is logged in
            if (IsUserSignedIn() == false)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            var user = _identity.GetApplicationUser(User);
            var indicatorsList = _indicatorListRepository.GetAll(user.Id);
            IndicatorListsViewModel model = new IndicatorListsViewModel()
            {
                IndicatorLists = Mapper.Map<IEnumerable<IndicatorListViewModel>>(indicatorsList)
            };
            return View("IndicatorListIndex", model);
        }

        [Route("create")]
        public ActionResult Create()
        {
            // Check user is logged in
            if (IsUserSignedIn() == false)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            ViewBag.Title = "Create new";
            IndicatorListViewModel indicator = new IndicatorListViewModel();
            return View("CreateIndicatorList", indicator);
        }

        [Route("save")]
        public ActionResult Save(IndicatorListViewModel indicatorListVM)
        {
            // Check user is logged in
            if (IsUserSignedIn() == false)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            var user = _identity.GetApplicationUser(User);

            // Check user owns the list
            if (HasIndicatorListIdBeenAssigned(indicatorListVM.Id) &&
                _indicatorListRepository.DoesUserOwnList(indicatorListVM.PublicId, user.Id) == false)
            {
                return GetJsonTaskResult(false, "You can only save lists that you have created");
            }

            indicatorListVM.UserId = user.Id;
            var indicatorList = Mapper.Map<IndicatorList>(indicatorListVM);
            var validationContext = new ValidationContext(indicatorList);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(indicatorList, validationContext, validationResults, true);
            if (!isValid)
            {
                return GetJsonTaskResult(false, validationResults.First().ErrorMessage);
            }

            SetSequencesOfIndicatorListItems(indicatorList);

            try
            {
                SaveIndicatorList(indicatorList, false);
            }
            catch (Exception ex)
            {
                _exceptionLoggerWrapper.LogException(ex, null);
                return GetJsonTaskResult(false, "Your indicator list could not be saved");
            }

            return GetJsonTaskResult(true, "Saved successfully");
        }

        [Route("copy")]
        public ActionResult Copy(string listId, string listName)
        {
            // Check user is logged in
            if (IsUserSignedIn() == false)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            // Check user owns the list
            var user = _identity.GetApplicationUser(User);
            if (_indicatorListRepository.DoesUserOwnList(listId, user.Id) == false)
            {
                return RedirectToAction("Index", "IndicatorList");
            }

            var indicatorList = new IndicatorList()
            {
                ListName = listName,
                UserId = user.Id
            };

            var indicatorListFromDb = _indicatorListRepository.GetListByPublicId(listId);
            foreach (var indicatorListItemFromDb in indicatorListFromDb.IndicatorListItems)
            {
                var indicatorListItem = new IndicatorListItem()
                {
                    AgeId = indicatorListItemFromDb.AgeId,
                    SexId = indicatorListItemFromDb.SexId,
                    IndicatorId = indicatorListItemFromDb.IndicatorId,
                    Sequence = indicatorListItemFromDb.Sequence
                };

                indicatorList.IndicatorListItems.Add(indicatorListItem);
            }

            try
            {
                SaveIndicatorList(indicatorList, true);
            }
            catch (Exception ex)
            {
                _exceptionLoggerWrapper.LogException(ex, null);
                ViewBag.Error = "Another indicator list already has that name. Please choose a different one.";
            }

            var indicatorsLists = _indicatorListRepository.GetAll(user.Id);
            IndicatorListsViewModel model = new IndicatorListsViewModel()
            {
                IndicatorLists = Mapper.Map<IEnumerable<IndicatorListViewModel>>(indicatorsLists)
            };
            return View("IndicatorListIndex", model);
        }

        [Route("edit")]
        public ActionResult Edit(string listId, bool redirect)
        {
            // Check user is logged in
            if (IsUserSignedIn() == false)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            // Check user owns the list
            var user = _identity.GetApplicationUser(User);
            if (_indicatorListRepository.DoesUserOwnList(listId, user.Id) == false)
            {
                return RedirectToAction("Index", "IndicatorList");
            }

            ViewBag.Title = "Edit";
            ViewBag.Redirect = redirect;
            var indicatorList = _indicatorListRepository.GetListByPublicId(listId);
            var indicatorListVm = Mapper.Map<IndicatorListViewModel>(indicatorList);
            return View("CreateIndicatorList", indicatorListVm);
        }

        [Route("delete")]
        public ActionResult Delete(string listId)
        {
            // Check user is logged in
            if (IsUserSignedIn() == false)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            if (string.IsNullOrWhiteSpace(listId) == false)
            {
                // Confirm user owns the list
                var user = _identity.GetApplicationUser(User);
                if (_indicatorListRepository.DoesUserOwnList(listId, user.Id))
                {
                    // Delete list
                    var list = _indicatorListRepository.GetListByPublicId(listId);
                    _indicatorListRepository.Delete(list.Id);
                }
            }
            return RedirectToAction("Index", "IndicatorList");
        }

        public ActionResult GetTopIndicatorList()
        {
            var usr = _identity.GetApplicationUser(User);
            var indicatorsList = _indicatorListRepository.GetTopIndicatorList(10, usr.Id);
            IndicatorListsViewModel model = new IndicatorListsViewModel()
            {
                IndicatorLists = Mapper.Map<IEnumerable<IndicatorListViewModel>>(indicatorsList)
            };
            return PartialView("_IndicatorListCollectionLinks", model);
        }

        private bool HasIndicatorListIdBeenAssigned(int id)
        {
            return id > 0;
        }

        private void SaveIndicatorList(IndicatorList indicatorList, bool createCopy = false)
        {
            if (!createCopy && HasIndicatorListIdBeenAssigned(indicatorList.Id))
            {
                _indicatorListRepository.Update(indicatorList);
            }
            else
            {
                // Assign ID on first save
                indicatorList.PublicId = _publicIdGenerator.GetIndicatorListPublicId();
                _indicatorListRepository.Save(indicatorList);
            }
        }

        private JsonResult GetJsonTaskResult(bool success, string message)
        {
            return new JsonResult
            {
                Data = new TaskResult
                {
                    Success = success,
                    Message = message
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static void SetSequencesOfIndicatorListItems(IndicatorList indicatorList)
        {
            int sequence = 0;
            foreach (var item in indicatorList.IndicatorListItems)
            {
                item.Sequence = sequence++;
            }
        }

        private bool IsUserSignedIn()
        {
            return _identity.IsUserSignedIn(User);
        }
    }
}