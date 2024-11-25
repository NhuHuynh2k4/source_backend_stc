using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public static class DatabaseConnection
{
    public static SqlConnection GetConnection(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return new SqlConnection(connectionString);
    }
}
