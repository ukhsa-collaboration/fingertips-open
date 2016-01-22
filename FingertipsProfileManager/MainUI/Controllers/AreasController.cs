using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;

namespace Fpm.MainUI.Controllers
{
    public class AreasController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private CoreDataRepository _coreDataRepository;

        public ActionResult ManageAreas(AreaGridModel model)
        {
            if (model.AreaGrid != null)
            {
                return View(model);
            }

            model = new AreaGridModel();
            return View(model);
        }

        public ActionResult ShowAreaDetails(string areaCode, string searchText, int areaTypeId)
        {
            var model = new AreaDetail();

            Area areaDetail = _coreDataRepository.GetAreaDetail(areaCode);
            model.AreaDetails = areaDetail;
            model.SearchAreaTypeId = areaTypeId;
            model.SearchText = searchText;

            return PartialView("_AreaDetail", model);
        }

        [HttpGet]
        public ActionResult SearchAreas(int areaTypeId, string searchText)
        {
            var model = new AreaGridModel {AreaGrid = new List<Area>()};

            HydrateGridFromDBLookup(model, searchText, areaTypeId);

            model.SearchAreaTypeId = areaTypeId;
            model.SearchText = searchText;

            return View("ManageAreas", model);
        }

        private void HydrateGridFromDBLookup(AreaGridModel model, string searchText, int areaTypeId)
        {
            int? areaTypeIdToSearch = null;
            if (areaTypeId > 0)
            {
                areaTypeIdToSearch = areaTypeId;
            }

            IEnumerable<Area> areas = CommonUtilities.GetAreas(searchText, areaTypeIdToSearch);

            PopulateAreaGrid(model, areas);
        }

        private static void PopulateAreaGrid(AreaGridModel model, IEnumerable<Area> areas)
        {
            foreach (Area area in areas)
            {
                model.AreaGrid.Add(new Area
                {
                    AreaCode = area.AreaCode,
                    AreaName = area.AreaName,
                    AreaTypeId = area.AreaTypeId,
                    AreaShortName = string.IsNullOrEmpty(area.AreaShortName) ? string.Empty : area.AreaShortName,
                    AddressLine1 = string.IsNullOrEmpty(area.AddressLine1) ? string.Empty : area.AddressLine1,
                    AddressLine2 = string.IsNullOrEmpty(area.AddressLine2) ? string.Empty : area.AddressLine2,
                    AddressLine3 = string.IsNullOrEmpty(area.AddressLine3) ? string.Empty : area.AddressLine3,
                    AddressLine4 = string.IsNullOrEmpty(area.AddressLine4) ? string.Empty : area.AddressLine4,
                    Postcode = string.IsNullOrEmpty(area.Postcode) ? string.Empty : area.Postcode,
                    IsCurrent = area.IsCurrent
                });
            }
        }

        [AuthorizedUsers]
        public ActionResult UpdateArea(AreaDetail model, string originalAreaCode)
        {
            var areaDetail = new Area
            {
                AreaCode = model.AreaDetails.AreaCode,
                AreaTypeId = model.AreaDetails.AreaTypeId,
                AreaName = model.AreaDetails.AreaName,
                AreaShortName = model.AreaDetails.AreaShortName,
                AddressLine1 = model.AreaDetails.AddressLine1,
                AddressLine2 = model.AreaDetails.AddressLine2,
                AddressLine3 = model.AreaDetails.AddressLine3,
                AddressLine4 = model.AreaDetails.AddressLine4,
                Postcode = model.AreaDetails.Postcode,
                IsCurrent = model.AreaDetails.IsCurrent
            };

            _coreDataRepository.UpdateAreaDetail(areaDetail, originalAreaCode, UserDetails.CurrentUser().Name);

            return RedirectToAction("SearchAreas",
                new {areaTypeId = model.SearchAreaTypeId, searchText = model.SearchText});
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _coreDataRepository = new CoreDataRepository();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
           
            _coreDataRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }
    }
}