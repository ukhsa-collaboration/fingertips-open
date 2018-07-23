
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Models
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