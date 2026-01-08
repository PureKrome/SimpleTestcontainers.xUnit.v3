namespace SimpleTestcontainers.xUnit.v3.DockerMonitor.Models;

internal class DatabaseInfo
{
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}
