using System.Configuration;

namespace Ckan.Client
{
    public class CkanApplicationConfiguration
    {
        public static string CkanRepositoryHostname
        {
            get { return ConfigurationManager.AppSettings["CkanRepositoryHostname"]; }
        }

        public static string CkanRepositoryApiKey
        {
            get { return ConfigurationManager.AppSettings["CkanRepositoryApiKey"]; }
        }
    }
}