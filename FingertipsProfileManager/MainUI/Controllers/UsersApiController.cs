using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Fpm.MainUI.Controllers
{
    public class UsersApiController : Controller
    {
        private readonly IProfilesReader _reader;
        private readonly IUserRepository _userRepository;

        public UsersApiController(IProfilesReader reader, IUserRepository userRepository)
        {
            _reader = reader;
            _userRepository = userRepository;
        }

        [Route("api/users")]
        public ActionResult All()
        {
            var json = JsonConvert.SerializeObject(_userRepository.GetAllFpmUsers());
            return Content(json, "application/json");
        }

        [Route("api/users/{id}")]
        public ActionResult GetUser(int id)
        {
            var user = _userRepository.GetAllFpmUsers().FirstOrDefault(x => x.Id == id);

            var json = JsonConvert.SerializeObject(user);
            return Content(json, "application/json");
        }
    }
}