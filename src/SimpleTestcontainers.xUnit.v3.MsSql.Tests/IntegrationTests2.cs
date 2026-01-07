namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests2(SqlServerFixtureWrapper SqlServerFixtureWrapper, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixtureWrapper, TestOutputHelper)
{    
}
