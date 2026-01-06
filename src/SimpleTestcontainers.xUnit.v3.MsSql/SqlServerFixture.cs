using Testcontainers.MsSql;
using WorldDomination.SimpleTestcontainers.xUnit.v3.Databases;
using Xunit;

namespace WorldDomination.SimpleTestcontainers.xUnit.v3.MsSql;

/// <summary>
/// Initializes a new instance of the <see cref="SqlServerFixture"/> class with some default settings.
///   - Container Name: SqlServer-Tests
///   - Image: mcr.microsoft.com/mssql/server:2022 (Refer to TestContainers for the specific version)
/// </summary>
public sealed class SqlServerFixture : ISimpleTestContainer, IDatabaseFixture, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;

    public SqlServerFixture(string? imageTag = null) =>
        _msSqlContainer = new MsSqlBuilder(imageTag ?? ImageTag)
            .WithName($"SqlServer-Tests-{Environment.Version}")
            .Build();

    /// <summary>
    /// The common connection string to the SqlServer instance. ** This has NOT been updated to be unique, per test **.
    /// </summary>
    public string ConnectionString => _msSqlContainer.GetConnectionString();

    public string ImageTag => "mcr.microsoft.com/mssql/server:2022-CU21-ubuntu-22.04";

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await _msSqlContainer.StartAsync();

    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await _msSqlContainer.StopAsync();
}
