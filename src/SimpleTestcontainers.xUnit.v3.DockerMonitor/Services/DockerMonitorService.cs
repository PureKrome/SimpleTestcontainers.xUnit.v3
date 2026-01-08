using Docker.DotNet;
using Docker.DotNet.Models;
using SimpleTestcontainers.xUnit.v3.DockerMonitor.Models;

namespace SimpleTestcontainers.xUnit.v3.DockerMonitor.Services;

internal class DockerMonitorService
{
    private readonly DockerClient _dockerClient;
    private readonly Dictionary<string, ContainerInfo> _containerHistory = [];

    public DockerMonitorService()
    {
        _dockerClient = new DockerClientConfiguration().CreateClient();
    }

    public Dictionary<string, ContainerInfo> ContainerHistory => _containerHistory;

    public async Task UpdateContainersAsync()
    {
        var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = false
        });

        var filteredContainers = containers
            .Where(c => c.Names.Any(name =>
                name.Contains("test-container", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("SqlServer-Tests", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("PostgreSql-Tests", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        MarkAllContainersAsInactive();

        foreach (var container in filteredContainers)
        {
            await UpdateContainerAsync(container);
        }
    }

    private void MarkAllContainersAsInactive()
    {
        foreach (var key in _containerHistory.Keys)
        {
            _containerHistory[key].IsActive = false;
        }
    }

    private async Task UpdateContainerAsync(ContainerListResponse container)
    {
        var containerId = container.ID;
        var containerName = container.Names.FirstOrDefault()?.TrimStart('/') ?? "Unknown";
        var imageName = container.Image;
        var status = container.Status;
        var ports = string.Join(", ", container.Ports.Select(p => $"{p.PublicPort}:{p.PrivatePort}"));

        if (!_containerHistory.ContainsKey(containerId))
        {
            _containerHistory[containerId] = new ContainerInfo
            {
                Id = containerId,
                Name = containerName,
                Image = imageName,
                FirstSeen = DateTime.Now
            };
        }

        var containerInfo = _containerHistory[containerId];
        containerInfo.IsActive = true;
        containerInfo.LastSeen = DateTime.Now;
        containerInfo.Status = status;
        containerInfo.Ports = ports;

        await UpdateContainerDatabasesAsync(container, containerInfo, containerName);
    }

    private static async Task UpdateContainerDatabasesAsync(
        ContainerListResponse container,
        ContainerInfo containerInfo,
        string containerName)
    {
        if (containerName.Contains("SqlServer-Tests", StringComparison.OrdinalIgnoreCase))
        {
            var databases = await SqlServerService.GetDatabasesAsync(container);
            DatabaseStateManager.UpdateDatabases(containerInfo, databases);
        }
        else if (containerName.Contains("PostgreSql-Tests", StringComparison.OrdinalIgnoreCase))
        {
            var databases = await PostgreSqlService.GetDatabasesAsync(container);
            DatabaseStateManager.UpdateDatabases(containerInfo, databases);
        }
    }
}
