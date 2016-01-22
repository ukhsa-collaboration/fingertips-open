using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PholioVisualisation.DataAccess
{
    public abstract class BaseDataAccess
    {
        public string ConnectionString { get; set; }

        protected SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        protected static string ReadString(SqlCommand cmd)
        {
            string s;
            try
            {
                cmd.Connection.Open();
                s = cmd.ExecuteScalar() as string;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return s;
        }

        protected static object ReadObject(SqlCommand cmd)
        {
            object o;
            try
            {
                cmd.Connection.Open();
                o = cmd.ExecuteScalar();
            }
            finally
            {
                cmd.Connection.Close();
            }
            return o;
        }

        protected static IList<string> GetStringList(DataTable table)
        {
            return (from DataRow row in table.Rows select (string)row.ItemArray[0]).ToList();
        }

        protected DataTable ExecuteSelectSqlString(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, GetConnection());
            return SelectToDataTable(cmd);
        }

        public static DataTable SelectToDataTable(SqlCommand cmd)
        {
            DataTable tbl = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter())
            {
                da.SelectCommand = cmd;
                try
                {
                    da.Fill(tbl);
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
            return tbl;
        }
    }
}