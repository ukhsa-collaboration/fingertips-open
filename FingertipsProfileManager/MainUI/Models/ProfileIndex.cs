using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Models
{
    public class ProfileIndex
    {
        public IEnumerable<ProfileDetails> Profiles { get; set; }
    }
}