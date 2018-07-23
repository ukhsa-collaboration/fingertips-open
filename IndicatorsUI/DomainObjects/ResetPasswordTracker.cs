using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndicatorsUI.DomainObjects
{
    [Table("ResetPasswordTracker")]
    public class ResetPasswordTracker
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestDateTime { get; set; }
        public bool IsRequestAccepted { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
}
