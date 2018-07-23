using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndicatorsUI.UserAccess.DataValidation.CustomValidation;

namespace IndicatorsUI.UserAccess
{
    
    [MetadataType(typeof(IndicatorListMetaData))]
    public partial class IndicatorList
    {
        
    }

    
    public class IndicatorListMetaData
    {
        [DuplicateNameCheck]
        [ListOwnerCheck]
        [MaxLength(100, ErrorMessage = "List name can not be longer than than 100 characters")]
        [Required(ErrorMessage = "List name is required")]
        public string ListName { get; set; }

        [Required]
        public string UserId { get; set; }

    }
}
