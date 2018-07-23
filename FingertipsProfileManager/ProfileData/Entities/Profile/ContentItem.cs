using System.ComponentModel.DataAnnotations;

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
        /// Is this content stored as plain text only.
        /// </summary>
        public bool IsPlainText { get; set; }

    }
}
