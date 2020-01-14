using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.UserFeedback;
using Fpm.ProfileData.Repositories;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("user-feedback")]
    public class UserFeedbackController : Controller
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;

        public UserFeedbackController(IUserFeedbackRepository userFeedbackRepository)
        {
            _userFeedbackRepository = userFeedbackRepository;
        }

        [Route]
        public ActionResult UserFeedbackIndex()
        {
            var feedbacks = _userFeedbackRepository.GetLatestUserFeedback(50).Where(x => x.HasBeenDealtWith == false);

            return View(feedbacks.ToList());
        }

        [HttpGet]
        [Route("item/{feedbackId}")]
        public ActionResult SingleUserFeedbackItem(int feedbackId)
        {
            var feedback = _userFeedbackRepository.GetFeedbackById(feedbackId);

            return View(feedback);
        }

        [HttpPost]
        [Route("item/{feedbackId}")]
        public ActionResult SingleUserFeedbackItem(UserFeedback userFeedback)
        {
            var feedbackFromDB = _userFeedbackRepository.GetFeedbackById(userFeedback.Id);
            if (userFeedback != null)
            {
                feedbackFromDB.Comment = userFeedback.Comment;
                _userFeedbackRepository.UpdateFeedback(feedbackFromDB);
            }
            return null;
        }


        [HttpPost]
        [Route("archive")]
        public ActionResult Archive(int id, string comment)
        {
            var user = UserDetails.CurrentUser();
            if (user.IsAdministrator == false)
            {
                throw new FpmException(
                    "Only admins are allowed to comment " + user.Name);
            }

            var feedback = _userFeedbackRepository.GetFeedbackById(id);
            if (feedback != null)
            {
                feedback.HasBeenDealtWith = true;
                feedback.Comment = comment;
                _userFeedbackRepository.UpdateFeedback(feedback);
            }
            return null;
        }

        [HttpPost]
        [Route("update")]
        public ActionResult Update(int id, string comment)
        {
            var user = UserDetails.CurrentUser();
            if (user.IsAdministrator == false)
            {
                throw new FpmException(
                    "Only admins are allowed to comment " + user.Name);
            }

            var feedback = _userFeedbackRepository.GetFeedbackById(id);
            if (feedback != null)
            {
                feedback.Comment = comment;
                _userFeedbackRepository.UpdateFeedback(feedback);
            }
            return null;
        }

    }
}