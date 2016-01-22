using System.Data.SqlClient;

namespace FingertipsBridgeWS.Cache
{
    public class JsonCacheManager : BaseDataAccess
    {
        private const string CacheKey = "cache_key";
        private const string ServiceId = "service_id";

        protected override SqlConnection Connection
        {
            get { return new SqlConnection(AppConfiguration.BridgeCacheConnectionString); }
        }

        public void SaveJson(JsonUnit jsonUnit)
        {
            SqlCommand cmd = GetProcedure("usp_pholio_insert_json");
            cmd.Parameters.AddWithValue(ServiceId, jsonUnit.ServiceId);
            cmd.Parameters.AddWithValue(CacheKey, jsonUnit.CacheKey);
            cmd.Parameters.AddWithValue("json", jsonUnit.Json);
            cmd.Parameters.AddWithValue("url", jsonUnit.Url);
            cmd.Parameters.AddWithValue("duration_ms", jsonUnit.DurationInMs);

            ExecuteNonSelectCommand(cmd);
        }

        public JsonUnit ReadJson(string serviceId, string cacheKey)
        {
            SqlCommand cmd = GetProcedure("usp_pholio_select_json");
            cmd.Parameters.AddWithValue(ServiceId, serviceId);
            cmd.Parameters.AddWithValue(CacheKey, cacheKey);
            byte[] data = ReadBytes(cmd);
            return new JsonUnit(serviceId, cacheKey, data);
        }

        private static byte[] ReadBytes(SqlCommand cmd)
        {
            byte[] data = null;
            try
            {
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data = (byte[])reader.GetValue(0);
                }
            }
            finally
            {
                cmd.Connection.Close();
            }
            return data;
        }
    }
}
