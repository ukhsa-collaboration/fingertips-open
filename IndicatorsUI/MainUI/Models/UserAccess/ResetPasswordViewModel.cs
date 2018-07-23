using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Models.UserAccess
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }

        [MaxLength(MaximumFieldLengths.Password)]
        [Required(ErrorMessage = "Password field is required and should not be empty")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "At least 8 characters and a minimum of 1 numeric character [0-9]")]
        public string NewPassword { get; set; }

        [MaxLength(MaximumFieldLengths.Password)]
        [Required(ErrorMessage = "Confirm Password field is required and should not be empty")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}