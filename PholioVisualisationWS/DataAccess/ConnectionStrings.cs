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

    /// <summary>
    /// Wrapper to allow Connection strings to be injected as dependency
    /// </summary>
    public class ConnectionStringsWrapper
    {
        public virtual string PholioConnectionString
        {
            get { return ConnectionStrings.PholioConnectionString; }
        }
    }
}