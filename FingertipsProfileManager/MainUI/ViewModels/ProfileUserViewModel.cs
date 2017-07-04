using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fpm.MainUI.ViewModels
{
    public class ProfileUserViewModel
    {
        public ProfileUserViewModel()
        {
            ProfileUsers = new List<ProfileUser>();
        }

        public IEnumerable<ProfileUser> ProfileUsers { get; set; }
        public int ProfileId { get; set; }
        public SelectList AllUsers { get; set; }
    }
}