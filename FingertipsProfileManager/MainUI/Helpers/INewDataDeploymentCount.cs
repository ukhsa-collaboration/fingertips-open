using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.Helpers
{
    public interface INewDataDeploymentCount
    {
        IList<SelectListItem> GetOptions(string selectedValue);
    }
}