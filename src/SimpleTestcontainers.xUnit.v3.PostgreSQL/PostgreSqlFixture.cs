using Testcontainers.PostgreSql;
using WorldDomination.SimpleTestcontainers.xUnit.v3.Databases;
using Xunit;

namespace WorldDomination.SimpleTestcontainers.xUnit.v3.PostgreSQL;

// <summary>
/// Initializes a new instance of the <see cref="SqlServerFixture"/> class with some default settings.
///   - Container Name: SqlServer-Tests
///   - Image: mcr.microsoft.com/mssql/server:2022-latest
/// </summary>
public sealed class PostgreSqlFixture : IDatabaseFixture, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithName("PostgreSql-Tests")
        .Build();

    /// <summary>
    /// The common connection string to the SqlServer instance. ** This has NOT been updated to be unique, per test **.
    /// </summary>
    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await _postgreSqlContainer.StartAsync();

    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await _postgreSqlContainer.StopAsync();
}

