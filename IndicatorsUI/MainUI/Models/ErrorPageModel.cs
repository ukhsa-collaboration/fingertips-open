
using Profiles.DataAccess;

namespace Profiles.MainUI.Models
{
    public class ErrorPageModel : PageModel
    {
        public ErrorPageModel(AppConfig appConfig)
            : base(appConfig)
        {

        }

        public Error Error { get; set; }
    }
}