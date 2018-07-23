using System.Linq;
using System.Text.RegularExpressions;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;

namespace PholioVisualisation.ServiceActions
{
    public class DeploymentStatusProvider
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly ConnectionStringsWrapper _connectionStrings;
        private readonly IDatabaseLogRepository _databaseLogRepository;

        public DeploymentStatusProvider(ApplicationConfiguration applicationConfiguration,
            ConnectionStringsWrapper connectionStrings, IDatabaseLogRepository databaseLogRepository)
        {
            _applicationConfiguration = applicationConfiguration;
            _connectionStrings = connectionStrings;
            _databaseLogRepository = databaseLogRepository;
        }

        public DeploymentStatus GetDeploymentStatus()
        {
            var status = new DeploymentStatus();

            // Data files
            var path = _applicationConfiguration.ExportFileDirectory;
            status.DataFiles = path.ToLower().EndsWith("a") ? "a" : "b";

            // Web site 
            var name = _applicationConfiguration.ApplicationName;
            status.WebSite = name.ToLower().EndsWith("a") ? "a" : "b";

            // Database
            var connectionString = _connectionStrings.PholioConnectionString;
            status.Database = connectionString.ToLower().Contains("pholio_live_a") ? "a" : "b";

            // More metadata
            status.DatabaseBackUpTimestamp = _databaseLogRepository.GetPholioBackUpTimestamp();
            status.BuildVersion = _applicationConfiguration.BuildVersion;

            return status;
        }
    }
}