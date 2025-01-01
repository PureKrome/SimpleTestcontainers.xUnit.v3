namespace SimpleTestcontainers.MsSql.Tests;

public class IntegrationTests1(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
