namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests6(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
