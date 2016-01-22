using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Fpm.ProfileData.Entities.Profile
{
    public class ContentItem 
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        
        public string Content { get; set; }
        
        [Required]
        [Display(Name = "Content Key")]
        public string ContentKey { get; set; }

        /// <summary>
        /// Only used to pass information to view. Is not stored in the database.
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        /// Only used to pass information to view. Is not stored in the database.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Only used to pass information to view. Is not stored in the database.
        /// </summary>
        public IEnumerable<SelectListItem> ProfileList { get; set; }

        /// <summary>
        /// Is this content stored as plain text only.
        /// </summary>
        public bool PlainTextContent { get; set; }

    }
}
