using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.Areas;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("areas")]
    public class AreasController : Controller
    {
        private readonly ICoreDataRepository _coreDataRepository;
        private IAreaTypeRepository _areaTypeRepository;

        public AreasController(ICoreDataRepository coreDataRepository, IAreaTypeRepository areaTypeRepository)
        {
            _coreDataRepository = coreDataRepository;
            _areaTypeRepository = areaTypeRepository;
        }

        [Route]
        public ActionResult AreasIndex(AreasIndexViewModel viewModel)
        {
            if (viewModel.AreaGrid == null)
            {
                viewModel = new AreasIndexViewModel();
            }
            return View(viewModel);
        }

        [Route("area-update")]
        [AdminUsersOnly]
        public ActionResult UpdateArea(AreaDetailViewModel model)
        {
            _coreDataRepository.UpdateAreaDetail(model.AreaDetails, model.InitialAreaCode, UserDetails.CurrentUser().Name);

            return RedirectToAction("SearchAreas",
                new { areaTypeId = model.SearchAreaTypeId, searchText = model.SearchText });
        }

        [Route("area-details")]
        public ActionResult ShowAreaDetails(string areaCode, string searchText, int areaTypeId)
        {
            var viewModel = new AreaDetailViewModel();

            Area areaDetail = _coreDataRepository.GetAreaDetail(areaCode);
            viewModel.InitialAreaCode = areaCode;
            viewModel.AreaDetails = areaDetail;
            viewModel.SearchAreaTypeId = areaTypeId;
            viewModel.SearchText = searchText;

            return PartialView("_AreaDetail", viewModel);
        }

        [Route("area-search")]
        [HttpGet]
        public ActionResult SearchAreas(int areaTypeId, string searchText)
        {
            // Clean search text
            searchText = searchText == null ? string.Empty : searchText.Trim();

            var viewModel = new AreasIndexViewModel {AreaGrid = new List<Area>()};

            HydrateGridFromDBLookup(viewModel, searchText, areaTypeId);

            viewModel.SearchAreaTypeId = areaTypeId;
            viewModel.SearchText = searchText;

            return View("AreasIndex", viewModel);
        }

        [Route("all-area-types")]
        public ActionResult GetAllAreaTypes()
        {
            _areaTypeRepository = new AreaTypeRepository(NHibernateSessionFactory.GetSession());
            var areaTypes = _areaTypeRepository.GetAllAreaTypes();

            return Json(areaTypes, JsonRequestBehavior.AllowGet);
        }

        private void HydrateGridFromDBLookup(AreasIndexViewModel viewModel, string searchText, int areaTypeId)
        {
            int? areaTypeIdToSearch = null;
            if (areaTypeId > 0)
            {
                areaTypeIdToSearch = areaTypeId;
            }

            IEnumerable<Area> areas = CommonUtilities.GetAreas(searchText, areaTypeIdToSearch);

            PopulateAreaGrid(viewModel, areas);
        }

        private static void PopulateAreaGrid(AreasIndexViewModel viewModel, IEnumerable<Area> areas)
        {
            foreach (Area area in areas)
            {
                viewModel.AreaGrid.Add(new Area
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
    }
}