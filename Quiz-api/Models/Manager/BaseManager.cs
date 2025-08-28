using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace Quiz_api.Models.Manager
{
    public class BaseManager
    {
        public static string ConnectionString { get; set; }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
