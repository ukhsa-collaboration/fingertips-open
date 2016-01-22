using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Fpm.ProfileData.Entities.Profile
{
    public class GroupingMetadata
    {
        public int GroupId { get; set; }

        [DisplayName("Domain Name")]
        [Required(ErrorMessage = "*")]
        [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
        public string GroupName { get; set; }

        public string Description { get; set; }

        [DisplayName("Sequence")]
        [Required(ErrorMessage = "*")]
        [Range(0, 99, ErrorMessage = "*")]
        public int Sequence { get; set; }
        public int ProfileId { get; set; }
    }
}
