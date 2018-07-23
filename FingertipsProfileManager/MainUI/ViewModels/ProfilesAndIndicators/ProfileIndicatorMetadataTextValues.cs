using System.Collections.Generic;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.ViewModels.ProfilesAndIndicators
{
    public class ProfileIndicatorMetadataTextValues
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public IList<IndicatorMetadataTextValue> IndicatorMetadataTextValues { get; set; }
    }
}