using IndicatorsUI.DomainObjects;
using System.ComponentModel.DataAnnotations;

namespace IndicatorsUI.MainUI.Models.UserAccess
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
            KeepUserLoggedIn = true;
        }

        [MaxLength(MaximumFieldLengths.EmailAddress)]
        [Required(ErrorMessage = "Email Address field is required and should not be empty")]
        public string UserName { get; set; }

        [MaxLength(MaximumFieldLengths.Password)]
        [Required(ErrorMessage = "Password field is required and should not be empty")]
        public string Password { get; set; }
        public bool KeepUserLoggedIn { get; set; }
        public string ReturnUrl { get; set; }
    }
}