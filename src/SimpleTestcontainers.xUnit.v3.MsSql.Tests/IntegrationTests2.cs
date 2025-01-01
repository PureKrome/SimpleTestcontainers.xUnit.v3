namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests2(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
