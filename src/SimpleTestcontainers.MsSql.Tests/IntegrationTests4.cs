namespace SimpleTestcontainers.MsSql.Tests;

public class IntegrationTests4(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
