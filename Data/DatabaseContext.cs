using Microsoft.Data.SqlClient;

namespace EmployeeManagementSystem.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext()
        {
            _connectionString =
                "Server=localhost;Database=EmployeeManagementDB;" +
                "Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public SqlConnection GetConnection() => new SqlConnection(_connectionString);
    }
}