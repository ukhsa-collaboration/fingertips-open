using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Helpers
{
    public interface IRemoveIndicatorChecker
    {
        bool CanIndicatorBeRemoved(int profileId, IndicatorMetadata indicatorMetadata, GroupingPlusName indicator);
    }
}