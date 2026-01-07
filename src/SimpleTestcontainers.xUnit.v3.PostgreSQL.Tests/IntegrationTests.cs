using Dapper;
using Npgsql;
using WorldDomination.SimpleTestcontainers.xUnit.v3.PostgreSQL;
using Xunit;

[assembly: AssemblyFixture(typeof(PostgreSqlFixtureWrapper))]
[assembly: CaptureConsole]

namespace SimpleTestcontainers.xUnit.v3.PostgreSQL.Tests;

public class IntegrationTests
{
    private readonly PostgreSqlFixture _postgreSqlFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public IntegrationTests(PostgreSqlFixtureWrapper PostgreSqlFixtureWrapper, ITestOutputHelper TestOutputHelper)
    {
        _postgreSqlFixture = PostgreSqlFixtureWrapper;
        _testOutputHelper = TestOutputHelper;
    }

    [Fact]
    public async Task QuerySingleAsync_GivenASimpleScript_ShouldReturn1()
    {
        // Act.

        // Lets create a unique database 'connection string' for this test.
        // NOTE: the actual database has NOT been created yet.
        var connectionStringInfo = _postgreSqlFixture.CreateUniqueConnectionStringInfo(TestContext.Current, _testOutputHelper);

        // Now that we know the name of the unique database we will use,
        // lets create this unique database.
        var createSql = $"CREATE DATABASE {connectionStringInfo.DatabaseName};";

        var generalConnectionString = _postgreSqlFixture.ConnectionString;

        using (var connection = new NpgsqlConnection(generalConnectionString))
        {
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            await connection.ExecuteAsync(createSql);
        }

        // Now that we have the unique database created, we can attempt to connect to it.
        const string querySql = "SELECT 1;";

        int result = 0;

        using (var connection = new NpgsqlConnection(connectionStringInfo.UniqueConnectionString))
        {
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            // Arrange.
            result = await connection.QuerySingleAsync<int>(querySql);
        }

        // Assert.
        Assert.Equal(1, result);
    }
}
