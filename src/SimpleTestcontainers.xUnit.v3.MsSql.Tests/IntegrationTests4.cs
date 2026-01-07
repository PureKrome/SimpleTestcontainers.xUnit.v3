namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests4(SqlServerFixtureWrapper SqlServerFixtureWrapper, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixtureWrapper, TestOutputHelper)
{    
}
