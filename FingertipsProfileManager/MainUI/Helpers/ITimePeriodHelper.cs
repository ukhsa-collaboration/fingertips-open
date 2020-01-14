using Fpm.ProfileData;

namespace Fpm.MainUI.Helpers
{
    public interface ITimePeriodHelper
    {
        string GetDatapointString();
        string GetBaselineString();
        string GetLatestPeriodString();
        TimePeriod GetPeriodIfLaterThanDatapoint();
    }
}