namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class IntegrationTests1(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
    : CommonIntegrationTests(SqlServerFixture, TestOutputHelper)
{    
}
