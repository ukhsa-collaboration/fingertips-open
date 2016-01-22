
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PholioVisualisation.DataAccess
{
    public class PholioLabelReader : BaseDataAccess
    {
        protected static SqlConnection GetPholioConnection
        {
            get { return new SqlConnection(ConnectionStrings.PholioConnectionString); }
        }

        public string LookUpAgeLabel(int ageId)
        {
            SqlCommand cmd = new SqlCommand("SELECT [Age] FROM [dbo].[L_Ages] WHERE [AgeID] = " + ageId,
                GetPholioConnection);
            return ReadString(cmd);
        }

        public string LookUpYearTypeLabel(int yearTypeId)
        {
            SqlCommand cmd = new SqlCommand("SELECT yeartype FROM L_YearTypes WHERE [yeartypeid] = " + yearTypeId,
                GetPholioConnection);
            return ReadString(cmd);
        }

        public string LookUpSexLabel(int sexId)
        {
            SqlCommand cmd = new SqlCommand("SELECT sex FROM L_Sexes WHERE [sexid] = " + sexId,
                GetPholioConnection);
            return ReadString(cmd);
        }

        public string LookUpComparatorMethodLabel(int comparatorMethodId)
        {
            SqlCommand cmd = new SqlCommand("SELECT comparatormethod FROM l_comparatormethods WHERE [comparatorMethodId] = " + comparatorMethodId,
                GetPholioConnection);
            return ReadString(cmd);
        }

        public string LookUpValueNoteLabel(int valueNoteId)
        {
            SqlCommand cmd = new SqlCommand("SELECT [Text] FROM [dbo].[L_ValueNotes] WHERE [ValueNoteId] = " + valueNoteId,
                GetPholioConnection);
            return ReadString(cmd);
        }


    }
}
