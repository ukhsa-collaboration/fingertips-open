using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Fpm.MainUI.ViewModels
{
    public class EditUserViewModel
    {
        public int ProfileId { get; set; }
        public int FpmUserId { get; set; }

        [Required(ErrorMessage = "required information")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "required information")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsMemberOfFpmSecurityGroup { get; set; }

        public SelectList ProfileList { get; set; }
    }
}