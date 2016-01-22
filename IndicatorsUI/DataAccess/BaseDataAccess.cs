using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Profiles.DataAccess
{
    public class BaseDataAccess
    {
        protected virtual SqlConnection Connection
        {
            get { throw new NotImplementedException("Connection string not set"); }
        }

        protected SqlCommand GetProcedure(string procedureName)
        {
            SqlCommand cmd = new SqlCommand(procedureName, Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        protected static void ExecuteNonSelectCommand(SqlCommand cmd)
        {
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        protected SqlCommand GetProcedureCommand(string procedureName,
            params KeyValuePair<string, object>[] parameters)
        {
            SqlCommand cmd = new SqlCommand(procedureName, Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
            return cmd;
        }

        protected int? ReadInt(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);

            int? i;
            try
            {
                cmd.Connection.Open();
                i = (int)cmd.ExecuteScalar();
            }
            finally
            {
                cmd.Connection.Close();
            }
            return i;
        }
    }
}
