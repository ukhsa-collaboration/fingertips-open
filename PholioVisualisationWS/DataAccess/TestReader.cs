
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PholioVisualisation.DataAccess
{
    /// <summary>
    /// Reader used solely for testing.
    /// </summary>
    public class TestReader : BaseDataAccess
    {
        public TestReader()
        {
            ConnectionString = ConnectionStrings.PholioConnectionString;
        }

        public List<int> GetLiveProfileIds()
        {
            DataTable table = ExecuteSelectSqlString("SELECT [id] FROM [dbo].[ui_Profiles] WHERE [exclude_indicators_from_search] = 0");
            return (from DataRow row in table.Rows select (int)row.ItemArray[0]).ToList();
        }
    }
}
