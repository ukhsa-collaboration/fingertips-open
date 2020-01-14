using Fpm.ProfileData;

namespace Fpm.MainUI.Helpers
{
    public interface ITimePeriodReader
    {
        string GetPeriodString(TimePeriod timePeriod, int yearTypeId);
    }
}