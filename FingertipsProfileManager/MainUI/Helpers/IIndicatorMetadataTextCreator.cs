using System.Collections.Generic;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Helpers
{
    public interface IIndicatorMetadataTextCreator
    {
        void CreateNewIndicatorTextValues(int profileId,
            IList<IndicatorMetadataTextItem> indicatorMetadataTextItems,
            IList<IndicatorMetadataTextProperty> properties,
            int nextIndicatorId, string userName);
    }
}