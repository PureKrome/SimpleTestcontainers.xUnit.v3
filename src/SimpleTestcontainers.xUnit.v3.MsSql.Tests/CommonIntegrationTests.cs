[assembly: AssemblyFixture(typeof(SqlServerFixture))]
[assembly: CaptureConsole]

namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public abstract class CommonIntegrationTests(SqlServerFixture SqlServerFixture, ITestOutputHelper TestOutputHelper)
{
    private static readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task ToListAsync_GivenAnExistingDatabase_ShouldReturnAllUsers()
    {
        // Arrange.
        var dbContext = await CommonTestHelpers.SetupDependenciesAsync(
            SqlServerFixture,
            TestContext.Current,
            TestOutputHelper);

        // Setup some fake data.
        var user = new User
        {
            FirstName = "Princess",
            LastName = "Leia",
            Email = "i.am.a.princess@example.com"
        };

        await dbContext.Users.AddAsync(user, _cancellationToken);
        await dbContext.SaveChangesAsync(_cancellationToken);

        // Act.
        var users = await dbContext.Users.ToListAsync(_cancellationToken);

        // Assert.
        Assert.NotNull(users);

        // FINALLY - lets delay the test to see that we can do this in parallel
        // based of xUnit's parallization via classes.
        await Task.Delay(5_000, _cancellationToken);
    }
}
