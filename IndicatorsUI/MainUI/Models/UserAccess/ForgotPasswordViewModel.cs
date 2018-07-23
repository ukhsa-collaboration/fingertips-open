using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Models.UserAccess
{
    public class ForgotPasswordViewModel
    {
            [MaxLength(MaximumFieldLengths.EmailAddress)]
            [Required(ErrorMessage = "Email address is required and should not be empty")]
            [EmailAddress(ErrorMessage = "This Email Address is not valid")]
            public string EmailAddress { get; set; }
    }
}