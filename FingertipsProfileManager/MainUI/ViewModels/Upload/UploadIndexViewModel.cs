using Fpm.MainUI.Helpers;

namespace Fpm.MainUI.ViewModels.Upload
{
    public class UploadIndexViewModel
    {
        public string BatchTemplateUrl { get; set; }
        public string SimpleTemplateUrl { get; set; }
        public string BatchLastUpdated { get; set; }
        public string SimpleLastUpdated { get; set; }
        public UserDetails User { get; set; }
    }
}