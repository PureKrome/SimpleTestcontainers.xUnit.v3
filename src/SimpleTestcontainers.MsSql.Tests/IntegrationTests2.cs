namespace SimpleTestcontainers.MsSql.Tests;

public class IntegrationTests2(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
