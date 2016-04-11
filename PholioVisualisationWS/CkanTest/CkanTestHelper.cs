using System.Configuration;
using System.IO;
using Ckan;
using Ckan.Client;
using Ckan.Model;

namespace PholioVisualisation.CkanTest
{
    public class CkanTestHelper
    {
        public static ICkanApi CkanApi()
        {
            var ckanApi = new CkanApi(new CkanHttpClient(
                         CkanApplicationConfiguration.CkanRepositoryHostname,
                         CkanApplicationConfiguration.CkanRepositoryApiKey));
            return ckanApi;
        }

        public static string GetExampleResponseFromFile(string fileName)
        {
            var path = ConfigurationManager.AppSettings["CkanExampleResponsesPath"];
            return File.ReadAllText(Path.Combine(path, fileName));
        }
    }
}