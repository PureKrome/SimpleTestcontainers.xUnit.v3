using Docker.DotNet.Models;
using Npgsql;

namespace SimpleTestcontainers.xUnit.v3.DockerMonitor.Services;

internal static class PostgreSqlService
{
    public static async Task<List<string>> GetDatabasesAsync(ContainerListResponse container)
    {
        var databases = new List<string>();

        try
        {
            var publicPort = container.Ports.FirstOrDefault(p => p.PrivatePort == 5432)?.PublicPort;
            if (publicPort == null)
            {
                return databases;
            }

            var connectionString = $"Host=localhost;Port={publicPort};Username=postgres;Password=postgres;Database=postgres;Timeout=5;";

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datistemplate = false AND datname != 'postgres' ORDER BY datname", connection);
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
