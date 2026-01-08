using SimpleTestcontainers.xUnit.v3.DockerMonitor.Services;

Console.WriteLine("Docker Container Monitor");
Console.WriteLine("========================");
Console.WriteLine("Monitoring containers matching: 'test-container', 'SqlServer-Tests', 'PostgreSql-Tests'");
Console.WriteLine("Press Ctrl+C to exit\n");

var monitorService = new DockerMonitorService();
var lastDisplayTime = DateTime.MinValue;
const int dataCollectionIntervalMs = 100;
const int displayRefreshIntervalMs = 1000;

while (true)
{
    try
    {
        await monitorService.UpdateContainersAsync();

        var timeSinceLastDisplay = (DateTime.Now - lastDisplayTime).TotalMilliseconds;
        if (timeSinceLastDisplay >= displayRefreshIntervalMs)
        {
            ConsoleDisplayService.DisplayContainers(monitorService.ContainerHistory);
            lastDisplayTime = DateTime.Now;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    await Task.Delay(dataCollectionIntervalMs);
}
