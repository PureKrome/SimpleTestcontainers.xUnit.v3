namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests3(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
