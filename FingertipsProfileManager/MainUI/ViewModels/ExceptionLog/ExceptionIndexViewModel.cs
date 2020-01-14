using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.ViewModels.ExceptionLog
{
    public class ExceptionIndexViewModel
    {
        public IList<ProfileData.Entities.Exceptions.ExceptionLog> ExceptionList { get; set; }
        public IEnumerable<SelectListItem> ServerList { get; set; }
        public string Server { get; set; }
        public bool LiveServersFilterApplied { get; set; }
        public bool ApiErrorsFilterApplied { get; set; }
        public int NumberOfDays { get; set; }
    }
}