using System.Web.Mvc;
using System.Collections.Generic;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;


namespace Fpm.MainUI.Models
{
    public class DocumentsGridModel
    {
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }

        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
        public int NumericPageCount { get; set; }

        public int ProfileId { get; set; }
        public IEnumerable<SelectListItem> ProfileList { get; set; }
        public IList<Document> DocumentItems { get; set; }
    }
}