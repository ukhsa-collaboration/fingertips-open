using System;
using System.IO;
using System.Web.Script.Serialization;

namespace Fpm.ProfileData.Entities.Job
{
    public class UploadJob
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public UploadJobStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public string DateCreatedF { get { return DateCreated.ToString("dd/MM/yyyy HH:mm"); } }
        public int UserId { get; set; }
        [ScriptIgnore]
        public string Username { get; set; }
        public int TotalRows { get; set; }
        public UploadJobType JobType { get; set; }
        [ScriptIgnore]
        public string Filename { get; set; }
        public string OriginalFile { get { return Path.GetFileName(Filename); } }
        public ProgressStage ProgressStage { get; set; }
    }
}
