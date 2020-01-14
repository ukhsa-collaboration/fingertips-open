using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class UserFeedbackController : ApiController
    {
        private UserFeedbackRepository _repo = new UserFeedbackRepository();

        /// <summary>
        /// Creates a new bit of user feedback
        /// </summary>
        /// <param name="userFeedback"></param>
        [HttpPost]
        [Route("user_feedback")]
        public void New(UserFeedback userFeedback)
        {
            if (userFeedback != null)
            {
                userFeedback.Timestamp = DateTime.Now;
                _repo.NewUserFeedback(userFeedback);
            }
        }

        /// <summary>
        /// Returns all the user feedbacks
        /// </summary>
        [HttpGet]
        [Route("user_feedback")]
        public IList<UserFeedback> Get()
        {
            var feedbacks = _repo.GetAllUserFeedbacks();
            return feedbacks.ToList();
        }


        /// <summary>
        /// Find user feedback by id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("user_feedback/{id}")]
        public UserFeedback FindById(int id)
        {
            var feedback = _repo.GetFeedbackById(id);
            return feedback;
        }
    }
}
