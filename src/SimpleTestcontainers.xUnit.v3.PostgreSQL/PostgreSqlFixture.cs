using Testcontainers.PostgreSql;
using WorldDomination.SimpleTestcontainers.xUnit.v3.Databases;
using Xunit;

namespace WorldDomination.SimpleTestcontainers.xUnit.v3.PostgreSQL;

// <summary>
/// Initializes a new instance of the <see cref="SqlServerFixture"/> class with some default settings.
///   - Container Name: SqlServer-Tests
///   - Image: https://hub.docker.com/layers/library/postgres/15.1/images/sha256-f25135c3038e660f35edd6bc752769b67fa0c43dec1f691069f8089f6c2fd7a0
/// </summary>
public sealed class PostgreSqlFixture : ISimpleTestContainer, IDatabaseFixture, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public PostgreSqlFixture(string? imageTag = null) =>
        _postgreSqlContainer = new PostgreSqlBuilder(imageTag ?? ImageTag)
            .WithName($"PostgreSql-Tests-{Environment.Version}")
            .Build();

    /// <summary>
    /// The common connection string to the SqlServer instance. ** This has NOT been updated to be unique, per test **.
    /// </summary>
    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    public string ImageTag => "postgres:18.1";

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await _postgreSqlContainer.StartAsync();

    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await _postgreSqlContainer.StopAsync();
}

