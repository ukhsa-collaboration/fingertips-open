using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Models
{
    public class AdminModel
    {
        public string AdminType { get; set; }
        public IEnumerable<ProfileDetails> Profiles { get; set; }
    }
}