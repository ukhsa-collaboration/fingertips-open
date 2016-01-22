using System.Configuration;

namespace PholioVisualisation.DataAccess
{
    public static class ConnectionStrings
    {
        public static string PholioConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["PholioConnectionString"].ConnectionString; }
        }
    }
}