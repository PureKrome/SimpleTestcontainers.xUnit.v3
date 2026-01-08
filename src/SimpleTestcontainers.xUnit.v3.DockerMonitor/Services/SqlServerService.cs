using Docker.DotNet.Models;
using Microsoft.Data.SqlClient;

namespace SimpleTestcontainers.xUnit.v3.DockerMonitor.Services;

internal static class SqlServerService
{
    public static async Task<List<string>> GetDatabasesAsync(ContainerListResponse container)
    {
        var databases = new List<string>();

        try
        {
            var publicPort = container.Ports.FirstOrDefault(p => p.PrivatePort == 1433)?.PublicPort;
            if (publicPort == null)
            {
                return databases;
            }

            var connectionString = $"Server=localhost,{publicPort};User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;Connection Timeout=5;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                databases.Add(reader.GetString(0));
            }
        }
        catch (Exception)
        {
            // Silently fail - container might not be ready yet
        }

        return databases;
    }
}
