using System.ComponentModel.DataAnnotations;

namespace Fpm.ProfileData.Entities.Core
{
    public class Area
    {
        [Required(ErrorMessage = "*")]
        [StringLength(20, ErrorMessage = "Area code be longer than 20 characters.")]
        public string AreaCode { get; set; }

        public string AreaName { get; set; }

        [StringLength(50, ErrorMessage = "Area short name be longer than 50 characters.")]
        public string AreaShortName { get; set; }
        
        public int AreaTypeId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public bool IsCurrent { get; set; }
        public string Postcode { get; set; }

    }
}
