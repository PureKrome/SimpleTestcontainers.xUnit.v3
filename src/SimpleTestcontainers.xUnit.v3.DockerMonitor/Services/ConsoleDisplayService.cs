using SimpleTestcontainers.xUnit.v3.DockerMonitor.Models;

namespace SimpleTestcontainers.xUnit.v3.DockerMonitor.Services;

internal static class ConsoleDisplayService
{
    public static void DisplayContainers(Dictionary<string, ContainerInfo> containerHistory)
    {
        Console.Clear();
        Console.WriteLine($"Last Update: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");

        if (!containerHistory.Any())
        {
            Console.WriteLine("No containers found yet.");
            return;
        }

        foreach (var containerInfo in containerHistory.Values.OrderByDescending(c => c.FirstSeen).ThenBy(c => c.Name))
        {
            DisplayContainer(containerInfo);
        }
    }

    private static void DisplayContainer(ContainerInfo containerInfo)
    {
        var statusIndicator = containerInfo.IsActive ? "ACTIVE" : "INACTIVE";
        var shortId = containerInfo.Id.Length > 12 ? containerInfo.Id[..12] : containerInfo.Id;

        Console.WriteLine($"Container: {containerInfo.Name} / ID: {shortId} / [{statusIndicator}]");
        Console.WriteLine($"  Image: {containerInfo.Image}");

        if (containerInfo.IsActive)
        {
            Console.WriteLine($"  Status: {containerInfo.Status}");
            Console.WriteLine($"  Ports: {containerInfo.Ports}");
        }
        else
        {
            Console.WriteLine($"  Last Seen: {containerInfo.LastSeen:yyyy-MM-dd HH:mm:ss}");
        }

        DisplayDatabases(containerInfo);

        Console.WriteLine();
    }

    private static void DisplayDatabases(ContainerInfo containerInfo)
    {
        if (!containerInfo.Databases.Any())
        {
            Console.WriteLine($"  Databases: (No databases found)");
            return;
        }

        var activeDbs = containerInfo.Databases.Count(d => d.IsActive);
        var inactiveDbs = containerInfo.Databases.Count(d => !d.IsActive);
        Console.WriteLine($"  Databases (Active: {activeDbs}, Inactive: {inactiveDbs}):");

        foreach (var db in containerInfo.Databases.OrderBy(d => d.FirstSeen).ThenBy(d => d.Name))
        {
            var dbStatus = db.IsActive ? "Active" : "Gone";
            Console.WriteLine($"    - {db.Name} ({dbStatus})");
        }
    }
}
