

using Fpm.ProfileData.Entities.Job;
using System.Collections.Generic;

namespace Fpm.MainUI.ViewModels.Upload
{
    public class UploadProgressViewModel
    {
        public int InProgress { get; set; }
        public int InQueue { get; set; }
        public int AwaitingConfomation { get; set; }
        public IEnumerable<UploadJob> Jobs { get; set; }
    }
}