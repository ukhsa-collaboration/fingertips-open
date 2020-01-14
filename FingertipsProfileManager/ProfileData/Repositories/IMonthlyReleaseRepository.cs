using System.Collections.Generic;
using Fpm.ProfileData.Entities.MonthlyRelease;

namespace Fpm.ProfileData.Repositories
{
    public interface IMonthlyReleaseRepository
    {
        IList<MonthlyRelease> GetUpcomingMonthlyReleases(int count);
    }
}