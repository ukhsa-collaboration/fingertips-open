using System.Collections.Generic;

namespace Fpm.MainUI.Helpers
{
    public class UploadHelper
    {
        public static bool AnyDisallowedIndicators(List<string> indicatorMessages)
        {
            return indicatorMessages != null && indicatorMessages.Count > 0;
        } 
    }
}