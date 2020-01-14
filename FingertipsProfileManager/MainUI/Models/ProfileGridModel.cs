using System.Collections.Generic;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Models
{
    public class ProfileGridModel
    {
        // Sorting-related properties
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }

        // Paging-related properties
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }

        // Filtering-related properties
        public IList<ProfileDetails> ProfileGrid { get; set; }
        public int UserId { get; set; }

        // Display-related properties
        public bool ShowNewProfileButton { get; set; }
    }
}