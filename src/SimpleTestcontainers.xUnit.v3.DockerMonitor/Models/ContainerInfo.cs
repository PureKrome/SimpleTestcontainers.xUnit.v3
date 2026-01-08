namespace SimpleTestcontainers.xUnit.v3.DockerMonitor.Models;

internal class ContainerInfo
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Image { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Ports { get; set; } = string.Empty;
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
    public List<DatabaseInfo> Databases { get; set; } = [];
}
