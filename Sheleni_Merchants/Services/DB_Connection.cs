using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Sheleni_Merchants.Services
{
    class DB_Connection
    {
        public SqlConnection Sheleni_Db_Connection()
        {
            string srvrdbname = "Sheleni";
            string srvrname = "172.20.10.5";
            string srvrusername = "developer";
            string srvrpassword = "%g1Dsesql&Gpjf6uPizB49q5JZ3NVquweW6$h8wxC";

            string sqlconn = $"Data Source={srvrname};Initial Catalog={srvrdbname};User ID={srvrusername};Password={srvrpassword}";
            SqlConnection sqlConnection = new SqlConnection(sqlconn);
            sqlConnection.Open();
            return sqlConnection;
        }
    }
}
