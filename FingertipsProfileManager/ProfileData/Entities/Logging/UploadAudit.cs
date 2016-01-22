using System;

namespace Fpm.ProfileData.Entities.Logging
{
    public class UploadAudit
    {
        public int Id { get; set; }
        public Guid UploadId { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedOn { get; set; }
        public int RowsUploaded { get; set; }
        public string UploadFilename { get; set; }
        public string WorksheetName { get; set; }
    }
}