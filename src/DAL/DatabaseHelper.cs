using System.Data.SqlClient;

namespace hoccsharp.DAL
{
    public static class DatabaseHelper
    {
        public static string ServerName = @".\MSSQLSERVER02";

        public static SqlConnection GetConnection(string dbName = "CafePOSDB")
        {
            string connString = $@"Data Source={ServerName};Initial Catalog={dbName};Integrated Security=True;TrustServerCertificate=True;";
            return new SqlConnection(connString);
        }
    }
}