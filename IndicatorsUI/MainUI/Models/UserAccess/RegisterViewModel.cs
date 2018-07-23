using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Models.UserAccess
{
    public class RegisterViewModel 
    {
        public string UserId { get; set; }

        [MaxLength(25)]
        [Required(ErrorMessage = "First name field is required and should not be empty")]
        [RegularExpression(@"^[a-záéíóúñ'ÑA-Z-~.’''|II|III|IV'\s]{1,40}$", ErrorMessage = "Special characters or numbers are not allowed. Please enter your First Name")]
        public string FirstName { get; set; }

        [MaxLength(25)]
        [Required(ErrorMessage = "Last name field is required and should not be empty")]
        [RegularExpression(@"^[a-záéíóúñ'ÑA-Z-~.’''|II|III|IV'\s]{1,40}$", ErrorMessage = "Special characters or numbers are not allowed. Please enter your Last Name")]
        public string LastName { get; set; }

        [MaxLength(MaximumFieldLengths.Password)]
        [Required(ErrorMessage = "Password field is required and should not be empty")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "At least 8 characters and a minimum of 1 numeric character [0-9]")]
        public string Password { get; set; }

        [MaxLength(MaximumFieldLengths.Password)]
        [Required(ErrorMessage = "Confirm Password field is required and should not be empty")]
        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Special characters or numbers are not allowed.")]
        public string JobTitle { get; set; }
        public int? OrganisationId { get; set; }

        /// <summary>
        /// User's email address (or PHE username on test site)
        /// </summary>
        [MaxLength(MaximumFieldLengths.EmailAddress)]
        [Required(ErrorMessage = "Email Address field is required and should not be empty")]
        [EmailAddress(ErrorMessage = "Enter valid Email Address")]
        public string UserName { get; set; }

        public bool ShowForgotPasswordLink { get; set; }
        public string ShowHideFlag
        {
            get
            {
                return ShowForgotPasswordLink ? "block" : "none";
            }
        }
        public Guid ? TempGuid { get; set; }
    }
}