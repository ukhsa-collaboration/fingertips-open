using Fpm.ProfileData.Entities.Job;

namespace Fpm.MainUI.ViewModels.Upload
{
    public class UploadQueueViewModel
    {
        public string DateCreatedF { get; set; }
        public string Username { get; set; }
        public string Filename { get; set; }
        public UploadJobStatus Status { get; set; }
        public string StatusText { get; set; }
    }
}