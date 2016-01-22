using System.Collections.Generic;
using System.Web.Mvc;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Models
{
    public class ExceptionGridModel
    {
        public IList<ExceptionLog> ExceptionGrid { get; set; }
        public IEnumerable<SelectListItem> ServerList { get; set; }

    }
}