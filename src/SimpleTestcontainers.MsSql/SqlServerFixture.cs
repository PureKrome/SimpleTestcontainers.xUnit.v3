using Testcontainers.MsSql;
using Xunit;

namespace WorldDomination.SimpleTestcontainers.MsSql;

/// <summary>
/// Initializes a new instance of the <see cref="SqlServerFixture"/> class with some default settings.
///   - Container Name: SqlServer-Tests
///   - Image: mcr.microsoft.com/mssql/server:2022-latest
/// </summary>
public sealed class SqlServerFixture: IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithName("SqlServer-Tests")
        .Build();

    /// <summary>
    /// The common connection string to the SqlServer instance. ** This has NOT been updated to be unique, per test **.
    /// </summary>
    public string ConnectionString => _msSqlContainer.GetConnectionString();

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await _msSqlContainer.StartAsync();

    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await _msSqlContainer.StopAsync();
}
