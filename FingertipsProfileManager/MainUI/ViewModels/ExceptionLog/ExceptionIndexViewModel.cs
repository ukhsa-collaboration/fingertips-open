using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.ViewModels.ExceptionLog
{
    public class ExceptionIndexViewModel
    {
        public IList<ProfileData.Entities.Exceptions.ExceptionLog> ExceptionList { get; set; }
        public IEnumerable<SelectListItem> ServerList { get; set; }
    }
}