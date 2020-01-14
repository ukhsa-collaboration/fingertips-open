using System.Collections.Generic;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Helpers
{
    public interface IIndicatorMetadataTextChanger
    {
        void UpdateIndicatorTextValues(int indicatorId, IList<IndicatorMetadataTextItem> textChangesByUser,
            IList<IndicatorMetadataTextProperty> properties, string userName, int profileId,
            bool isOwnerProfileBeingEdited);
    }
}