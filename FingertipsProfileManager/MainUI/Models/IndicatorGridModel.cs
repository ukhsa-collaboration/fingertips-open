using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Fpm.ProfileData;

namespace Fpm.MainUI.Models
{
    public class IndicatorGridModel : BaseDataModel
    {
        public IndicatorGridModel()
        {
            PageSize = 200;
            NumericPageCount = 15;
        }


        // Sorting-related properties
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }
        public string SortExpression
        {
            get
            {
                return SortAscending ? SortBy + " asc" : SortBy + " desc";
            }
        }


        // Paging-related properties
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
        public int PageCount
        {
            get
            {
                return Math.Max((TotalRecordCount + PageSize - 1) / PageSize, 1);
            }
        }
        public int NumericPageCount { get; set; }

        // Filtering-related properties
        public string ProfileKey { get; set; }
        public string ContactUserName { get; set; }
        public string EmailAddress { get; set; }
        public int? DomainSequence { get; set; }
        public int? IndicatorId { get; set; }
        public int SelectedAreaTypeId { get; set; }
        public int SelectedGroupId { get; set; }
        public string IndicatorText { get; set; }
        public IEnumerable<SelectListItem> ProfileList { get; set; }
        public IEnumerable<SelectListItem> DomainList { get; set; }
        public IEnumerable<SelectListItem> AreaTypeList { get; set; }

        public IEnumerable<GroupingPlusName> IndicatorNamesGrid { get; set; }

    }
}