using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.Upload;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    public class UploadController : Controller
    {
        /// <summary>
        ///  Upload Index
        /// </summary>
        [Route("upload")]
        public ActionResult UploadIndex()
        {
            const string batchTemplateRelativePath = "/upload-templates/indicator-upload-template.xlsx";

            var viewModel = new UploadIndexViewModel
            {
                BatchTemplateUrl = batchTemplateRelativePath,
                BatchLastUpdated = AppConfig.LastUpdatedDateBatchTemplate,
                User = UserDetails.CurrentUser()
            };

            return View(viewModel);
        }
    }
}