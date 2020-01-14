using Fpm.ProfileData.Entities.Logging;

namespace Fpm.ProfileData.Repositories
{
    public interface ILoggingRepository
    {
        DatabaseLog GetDatabaseLog(int databaseLogId);
    }
}