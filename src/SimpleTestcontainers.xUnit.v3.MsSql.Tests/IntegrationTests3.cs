namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests3(SqlServerFixtureWrapper SqlServerFixtureWrapper, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixtureWrapper, TestOutputHelper)
{    
}
