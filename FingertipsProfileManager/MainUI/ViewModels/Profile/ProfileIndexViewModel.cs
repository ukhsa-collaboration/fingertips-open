
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fpm.MainUI.ViewModels.Profile
{
    /// <summary>
    /// View model for non-admin profile edit page
    /// </summary>
    public class ProfileIndexViewModel
    {
        /// <summary>
        /// Selected profile ID
        /// </summary>
        public int ProfileId { get; set; }

        /// <summary>
        /// List of all the profiles the user has permission to
        /// </summary>
        public SelectList ProfileList { get; set; }

        /// <summary>
        /// View model of specific profile to be edited
        /// </summary>
        public ProfileViewModel ProfileViewModel { get; set; }
    }
}