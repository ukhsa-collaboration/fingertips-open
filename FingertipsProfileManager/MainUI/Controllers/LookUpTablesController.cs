using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    public class LookUpTablesController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();
        private LookUpsRepository _lookUpsRepository;

        public ActionResult Index()
        {
            return View("LookUpTablesIndex");
        }

        public ActionResult Sexes()
        {
            var model = new LookupModel
            {
                LookupType = "Sexes",
                Sexes = _reader.GetAllSexes()
            };
            return View("LookUpList", model);
        }

        public ActionResult Ages()
        {
            var model = new LookupModel
            {
                LookupType = "Ages",
                Ages = _reader.GetAllAges()
            };
            return View("LookUpList", model);
        }

        public ActionResult ValueNotes()
        {
            var model = new LookupModel
            {
                LookupType = "Value notes",
                ValueNotes = _reader.GetAllValueNotes()
            };
            return View("LookUpList", model);
        }

        [AcceptVerbs("Get", "Post")]
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

        public ActionResult Targets()
        {
            var model = new LookupModel
            {
                LookupType = "Goals",
                Targets = _reader.GetAllTargets().OrderBy(x => x.LowerLimit).ToList(),
                Polarities = _reader.GetAllPolarities()
            };
            return View("LookUpList", model);
        }

        public ActionResult TargetEdit(int targetId)
        {
            var model = new TargetEditModel
            {
                Target = _reader.GetTargetById(targetId)
            };
            return View("TargetEdit", model);
        }

        [HttpPost]
        public ActionResult TargetEdit(TargetEditModel model, string button)
        {
            var userId = UserDetails.CurrentUser().Id;
            var target = model.Target;

            if (button.ToLower().StartsWith("delete"))
            {
                if (UserDetails.CurrentUser().IsAdministrator)
                {
                    // Audit state in database before deletion
                    var targetFromDatabase = _reader.GetTargetById(target.Id);
                    _writer.DeleteTargetConfig(target);
                    _writer.NewTargetConfigAudit(new TargetConfigAudit(targetFromDatabase, userId, "Deleted"));
                }
            }
            else
            {
                _writer.UpdateTargetConfig(model.Target);
                _writer.NewTargetConfigAudit(new TargetConfigAudit(model.Target, userId, "Updated"));
            }
            return Redirect(SiteUrls.TargetIndex);
        }

        public ActionResult TargetNew()
        {
            var model = new TargetEditModel
            {
                Target = new TargetConfig()
            };
            return View("TargetEdit", model);
        }

        [HttpPost]
        public ActionResult TargetNew(TargetEditModel model)
        {
            _writer.NewTargetConfig(model.Target);
            _writer.NewTargetConfigAudit(new TargetConfigAudit(model.Target, UserDetails.CurrentUser().Id, "New"));
            return Redirect(SiteUrls.TargetIndex);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_lookUpsRepository == null) _lookUpsRepository = new LookUpsRepository();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _lookUpsRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }

    }
}