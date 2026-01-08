using SimpleTestcontainers.xUnit.v3.DockerMonitor.Models;

namespace SimpleTestcontainers.xUnit.v3.DockerMonitor.Services;

internal static class DatabaseStateManager
{
    public static void UpdateDatabases(ContainerInfo containerInfo, List<string> currentDatabases)
    {
        // Mark all databases as inactive first
        foreach (var db in containerInfo.Databases)
        {
            db.IsActive = false;
        }

        // Update or add databases
        foreach (var dbName in currentDatabases)
        {
            var existingDb = containerInfo.Databases.FirstOrDefault(d => d.Name == dbName);
            if (existingDb != null)
            {
                existingDb.IsActive = true;
                existingDb.LastSeen = DateTime.Now;
            }
            else
            {
                containerInfo.Databases.Add(new DatabaseInfo
                {
                    Name = dbName,
                    IsActive = true,
                    FirstSeen = DateTime.Now,
                    LastSeen = DateTime.Now
                });
            }
        }
    }
}
