using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Models
{
    public class ProfileCollectionGridModel
    {
        public ProfileCollectionGridModel()
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

        // Filtering-related properties
        public int ProfileId { get; set; }
        public IEnumerable<SelectListItem> ProfileList { get; set; }
        public IList<ProfileCollection> ProfileCollectionGrid { get; set; }
    }
}