using System;

namespace FingertipsUploadService.ProfileData.Entities.Logging
{
    public class IndicatorMetadataLog
    {
        public int Id { get; set; }
        
        public int IndicatorId { get; set; }

        public int GroupId { get; set; }

        public int PropertyId { get; set; }

        public string OldText { get; set; }

        public DateTime Timestamp { get; set; }

        public string UserName { get; set; }

    }
}
