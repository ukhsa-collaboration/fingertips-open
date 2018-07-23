using Fpm.ProfileData.Repositories;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    public class UserFeedbackController : Controller
    {
        private readonly UserFeedbackRepository _userFeedbackRepository = new UserFeedbackRepository();
        public ActionResult Index()
        {
            var feedbacks = _userFeedbackRepository.GetLatestUserFeedback(50);
            return View(feedbacks.ToList());
        }
    }
}