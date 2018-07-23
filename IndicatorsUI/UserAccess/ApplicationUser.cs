using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace IndicatorsUI.UserAccess
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string JobTitle { get; set; }

        [Required]
        public bool HasUserValidatedEmailAccount { get; set; }

        public int? OrganisationId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string AccessTokenResetPassword { get; set; }

        /// <summary>
        /// UID used for temporary links sent to the user.
        /// </summary>
        public Guid? TempGuid { get; set; }
    }
}
