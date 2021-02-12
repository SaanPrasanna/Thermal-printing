using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CustomJson
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "csharptest";
            string username = "root";
            string password = "";

            return DBMySqlUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
