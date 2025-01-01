using Microsoft.EntityFrameworkCore;
using WorldDomination.SimpleTestcontainers.MsSql;
using Xunit;

namespace SimpleTestcontainers.MsSql.Tests;

internal static class CommonTestHelpers
{
    internal static async ValueTask<SqlServerDbContext> SetupDependenciesAsync(
        SqlServerFixture testsFixture,
        ITestContext testContext,
        ITestOutputHelper testOutputHelper)
    {
        // Grab our unique connection string.
        var connectionString = testsFixture.CreateDbConnectionString(testContext, testOutputHelper);

        // Wire up our EF Core DbContext to this unique connection string.
        var efOptions = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        // Create our context.
        var dbContext = new SqlServerDbContext(efOptions);

        // Create the dabase if it doesn't exist (which it shouldn't)
        var isCreated = await dbContext.Database.EnsureCreatedAsync(testContext.CancellationToken);

        // Safety check: we want to make sure the unique database was created!
        if (!isCreated)
        {
            throw new InvalidOperationException("The database already exists and was not created.");
        }

        return dbContext;
    }
}
