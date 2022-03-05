using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace BOM.Utility
{
    public class DataAccess
    {
        public string conString = "Data Source=JXKNIGHTDESKTOP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public SqlDataReader queryDB(string sql,SqlCommand cmd)
        {
            SqlConnection conn = new SqlConnection(conString);
            cmd.Connection = conn;
            cmd.CommandText = sql;
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }

        public bool execQuery(string sql,SqlCommand cmd)
        {
            SqlConnection conn = new SqlConnection(conString);
            cmd.Connection = conn;
            cmd.CommandText = sql;
            conn.Open();
            if(cmd.ExecuteNonQuery() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}