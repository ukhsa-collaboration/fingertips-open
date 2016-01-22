using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FingertipsUploadService.ProfileData.Entities.Profile
{
    public class ProfileCollection
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "CollectionName")]
        public string CollectionName { get; set; }

        [Required]
        [Display(Name = "CollectionSkinTitle")]
        public string CollectionSkinTitle { get; set; }

        public string ReturnUrl { get; set; }

        public IList<ProfileCollectionItem> ProfileCollectionItems { get; set; }
    }
}
