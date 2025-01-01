namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests5(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
