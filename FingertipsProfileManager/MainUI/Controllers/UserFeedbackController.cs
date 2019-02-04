using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("userfeedback")]
    public class UserFeedbackController : Controller
    {
        private readonly UserFeedbackRepository _userFeedbackRepository = new UserFeedbackRepository();
        [Route("")]
        public ActionResult Index()
        {
            var feedbacks = _userFeedbackRepository.GetLatestUserFeedback(50).Where(x => x.HasBeenDealtWith == false);

            return View(feedbacks.ToList());
        }

        [HttpPost]
        [Route("archive")]
        public ActionResult Archive(int id, string comment)
        {
            var user = UserDetails.CurrentUser();
            if (user.IsAdministrator == false)
            {
                throw new FpmException(
                    "Only admins are allowed to commnet " + user.Name);
            }

            var feedback = _userFeedbackRepository.GetFeedbackById(id);
            if (feedback != null)
            {
                feedback.HasBeenDealtWith = true;
                feedback.Comment = comment;
                _userFeedbackRepository.UdateFeedback(feedback);
            }
            return null;
        }
    }
}