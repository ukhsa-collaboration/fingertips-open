using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.ActionFilter;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("lookup-tables")]
    public class LookUpTablesController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();
        private LookUpsRepository _lookUpsRepository;

        [Route("")]
        public ActionResult Index()
        {
            return View("LookUpTablesIndex");
        }

        [Route("sexes")]
        public ActionResult Sexes()
        {
            var model = new LookupModel
            {
                LookupType = "Sexes",
                Sexes = _reader.GetAllSexes()
            };
            return View("LookUpList", model);
        }

        [Route("ages")]
        public ActionResult Ages()
        {
            var model = new LookupModel
            {
                LookupType = "Ages",
                Ages = _reader.GetAllAges()
            };
            return View("LookUpList", model);
        }

        [Route("value-notes")]
        public ActionResult ValueNotes()
        {
            var model = new LookupModel
            {
                LookupType = "Value notes",
                ValueNotes = _reader.GetAllValueNotes()
            };
            return View("LookUpList", model);
        }

        [Route("area-types")]
        public ActionResult AreaTypes()
        {
            var model = new LookupModel
            {
                LookupType = "Area types",
                AreaTypes = new AreaTypeRepository().GetAllAreaTypes().Where(x => x.IsCurrent).ToList()
            };
            return View("LookUpList", model);
        }

        [Route("area-type/edit/{areaTypeId}")]
        [HttpGet]
        public ActionResult EditAreaType(int areaTypeId)
        {
            var areaType = new AreaTypeRepository().GetAreaType(areaTypeId);
            areaType.ComponentAreaTypesString = areaType.GetComponentAreaTypesAsString();
            return View("EditAreaType", areaType);
        }

        [Route("area-type/edit/{areaTypeId}")]
        [HttpPost]
        public ActionResult EditAreaType(AreaType areaType)
        {
            areaType.ParseComponentAreaTypesString();
            var repo = new AreaTypeRepository();

            // Copy properties to persistable area type
            var areaTypeFromDb = repo.GetAreaType(areaType.Id);
            AutoMapper.Mapper.Map(areaType, areaTypeFromDb);
            areaTypeFromDb.ReplaceComponentAreaTypes(areaType.ComponentAreaTypes);

            repo.SaveAreaType(areaTypeFromDb);
            return Redirect(SiteUrls.AreaTypeIndex);
        }

        [AcceptVerbs("Get", "Post")]
        [Route("categories")]
        public ActionResult Categories(int categoryTypeId = CategoryTypeIds.EthnicGroups7Categories)
        {
            var categoryTypes = _lookUpsRepository.GetCategoryTypes()
                .OrderBy(x => x.Name).ToList();

            var model = new LookupModel
            {
                LookupType = "Categories",
                CategoryTypes = categoryTypes,
                CategoryTypeId = categoryTypeId,
                Categories = _reader.GetCategoriesByCategoryTypeId(categoryTypeId)
            };
            return View("LookUpList", model);
        }

        [AcceptVerbs("Get", "Post")]
        [Route("edit-category")]
        public ActionResult EditCategory(int categoryTypeId)
        {
            var categoryTypes = _lookUpsRepository.GetCategoryTypes()
                .OrderBy(x => x.Name).ToList();

            var categoryType = categoryTypes.FirstOrDefault(x => x.Id == categoryTypeId);

            var model = new LookupModel
            {
                LookupType = "Categories",
                CategoryTypes = categoryTypes,
                CategoryTypeId = categoryTypeId,
                Categories =  _reader.GetCategoriesByCategoryTypeId(categoryTypeId)
            };

            return View("EditCategoryType", model);
        }

        [Route("update-category")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateCategory(CategoryType categoryType)
        {
            var existingCategoryType = new CategoryType();
            AutoMapper.Mapper.Map(categoryType, existingCategoryType);

            _writer.UpdateCategoryType(categoryType);

            return RedirectToAction("Categories", "LookUpTables", new {categoryTypeId = categoryType.Id});
        }

        [Route("targets")]
        public ActionResult TargetIndex()
        {
            var model = new LookupModel
            {
                LookupType = "Goals",
                Targets = _reader.GetAllTargets().OrderBy(x => x.LowerLimit).ToList(),
                Polarities = _reader.GetAllPolarities()
            };
            return View("LookUpList", model);
        }

        [HttpGet]
        [Route("target/edit")]
        public ActionResult TargetEdit(int targetId)
        {
            var model = new TargetEditModel
            {
                Target = _reader.GetTargetById(targetId)
            };
            return View("TargetEdit", model);
        }

        [HttpPost]
        [Route("target/edit")]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult TargetSave(TargetEditModel model)
        {
            var userId = UserDetails.CurrentUser().Id;

            if (ModelState.IsValid)
            {
                _writer.UpdateTargetConfig(model.Target);
                _writer.NewTargetConfigAudit(new TargetConfigAudit(model.Target, userId, "Updated"));
                return Redirect(SiteUrls.TargetIndex);
            }

            return View("TargetEdit", model);
        }

        [HttpPost]
        [Route("target/edit")]
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult TargetDelete(TargetEditModel model)
        {
            var userId = UserDetails.CurrentUser().Id;
            var target = model.Target;

            if (UserDetails.CurrentUser().IsAdministrator)
            {
                // Audit state in database before deletion
                var targetFromDatabase = _reader.GetTargetById(target.Id);
                _writer.DeleteTargetConfig(target);
                _writer.NewTargetConfigAudit(new TargetConfigAudit(targetFromDatabase, userId, "Deleted"));
            }

            return Redirect(SiteUrls.TargetIndex);
        }

        [Route("target/new")]
        public ActionResult TargetNew()
        {
            var model = new TargetEditModel
            {
                Target = new TargetConfig()
            };
            return View("TargetEdit", model);
        }

        [HttpPost]
        [Route("target/new")]
        public ActionResult TargetNew(TargetEditModel model)
        {
            _writer.NewTargetConfig(model.Target);
            _writer.NewTargetConfigAudit(new TargetConfigAudit(model.Target, UserDetails.CurrentUser().Id, "New"));
            return Redirect(SiteUrls.TargetIndex);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_lookUpsRepository == null) _lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _lookUpsRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }

    }
}