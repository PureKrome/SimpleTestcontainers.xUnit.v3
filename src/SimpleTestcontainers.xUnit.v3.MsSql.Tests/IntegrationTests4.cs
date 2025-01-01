namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests4(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
