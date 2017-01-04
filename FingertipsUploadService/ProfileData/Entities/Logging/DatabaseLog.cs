using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingertipsUploadService.ProfileData.Entities.Logging
{
    public class DatabaseLog
    {
        public int Id { get; set; }
        public string Event { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
