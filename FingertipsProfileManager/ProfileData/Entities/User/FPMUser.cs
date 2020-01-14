using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Fpm.ProfileData.Entities.User
{
    public class FpmUser
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress {
            get { return new Regex("^phe.").Replace(UserName.ToLower(), "") + "@phe.gov.uk"; } 
        }

        public bool IsAdministrator { get; set; }
        public bool IsReviewer { get; set; }
        public bool IsCurrent { get; set; }
    }
}