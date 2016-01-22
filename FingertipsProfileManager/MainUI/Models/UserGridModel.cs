using System.Collections.Generic;
using System.Web.Mvc;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.User;

namespace Fpm.MainUI.Models
{
    public class UserGridModel
    {
        public UserGridModel()
        {
            PageSize = 100;
            NumericPageCount = 15;
        }

        // Sorting-related properties
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }

        // Paging-related properties
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
        public int NumericPageCount { get; set; }

        public IEnumerable<FpmUser> UserGrid { get; set; }
    }
}