<h1 align="center">Simple: Database Testcontainer Helpers for xUnit v3</h1>

<div align="center">
  ⚡ Making it simple to run your database tests in true database-isolation, quickly! ⚡
</div>

<br />

<div align="center">
    <!-- License -->
    <a href="https://choosealicense.com/licenses/mit/">
    <img src="https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square" alt="License - MIT" />
    </a>
    <!-- NuGet -->
    <a href="https://www.nuget.org/packages/WorldDomination.SimpleTestcontainers.MsSql/">
    <img src="https://buildstats.info/nuget/WorldDomination.SimpleTestcontainers.MsSql" alt="NuGet" />
    </a>
</div>


---
## Overview

Testing code that depends on a database can be slow and brittle.

**This library aims to make it simple and fast to test your code that depends on a database.**

[Testcontainers](https://dotnet.testcontainers.org/) are a great way to test your code that depends on a database.
It can also be slow with the common xUnit pattern of using a "Test Collection" which means each test in that 'collection' is sequential.
It's also annoying when creating lots of xUnit ceremony when making multiple collections.
Finally, it's frustrating that multiple tests hitting the **same Database** can cause the tests to be brittle.

**This library aims to make it simple to test your code that depends on a database:**

- ✅ A single database per (xUnit) test class.
- ✅ Tests are now isolated from each other.
- ⚡ The entire test run can be faster because they now run in parallel (based off xUnit's [Test Collections concept](https://xunit.net/docs/running-tests-in-parallel#parallelism-in-test-frameworks))

---

## Key Differences vs. Standard TestContainers

| Feature | Standard TestContainers | This Library |
|---------|------------------------|--------------|
| **Parallel Test Execution** | ❌ Requires Test Collections (sequential) | ✅ Test classes run in parallel by default |
| **Database Isolation** | ❌ Shared database = data conflicts | ✅ Unique database per test class |
| **Setup Complexity** | ❌ Manual Test Collection attributes | ✅ Zero ceremony - just inject the fixture |
| **Performance** | ❌ Usually slower because of sequential execution via Test Collection | ✅ Faster (parallel across CPU cores) but does depend on how much data you _seed_ on each isolated db that is created |
| **Container Usage** | ✅ One container, one DB | ✅ One container, multiple databases |
| **Test Independence** | ❌ Test _data_ can interfere with each other (need to rollback changes, etc) | ✅ Complete isolation between test classes |

### How It Works

**Standard Approach:**
```csharp
// ❌ Forces sequential execution
[Collection("Database Collection")]
public class UserTests : IClassFixture<DatabaseContainer>
{
    [Fact]
    public async Task Test1() { /* Shared DB */ }
}
```

**This Library:**
```csharp
// ✅ Runs in parallel automatically
public class UserTests1(SqlServerFixture fixture, ITestOutputHelper output)
{
    [Fact]
    public async Task Test()
    {
        // Gets unique DB: "usertest1_abc123"
        var connectionString = fixture.CreateDbConnectionString(TestContext.Current, output);
        // ...
    }
}

public class UserTests2(SqlServerFixture fixture, ITestOutputHelper output)
{
    [Fact]
    public async Task Test()
    {
        // Gets unique DB: "usertest2_def456" 
        var connectionString = fixture.CreateDbConnectionString(TestContext.Current, output);
        // ...
    }
}
// Both classes run IN PARALLEL! 🚀
```

**The Magic:** The `CreateDbConnectionString()` method generates a unique database name per test class (using test name + GUID), allowing the same container to host multiple isolated databases and enabling parallel test execution without data conflicts.

---

## 💻 TL;DR; Show me some code!

### A bare minimum example.
```csharp
public class SomeTests(
    SqlServerFixture SqlServerFixture, // The MSSql Testcontainer. Either this simple one or your own custom one.
    ITestOutputHelper TestOutputHelper // xUnit's output helper.
)
{
    private static readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task ToListAsync_GivenAnExistingDatabase_ShouldReturnAllUsers()
    {
        // Arrange.
        // Grab our unique connection string.
        var connectionString = SqlServerFixture.CreateDbConnectionString(TestContext.Current, TestOutputHelper);

        using (var db = new SqlConnection(connectionString))
        {
            // Act.
            var users = await db.QueryAsync<User>("SELECT * FROM Users", cancellationToken: _cancellationToken).ToListAsync(_cancellationToken);
        }

        // Assert.
        ....
    }
```

### Using a custom Docker image tag.
```csharp
public class SomeTests(
    SqlServerFixture SqlServerFixture, // The MSSql Testcontainer with a custom image tag.
    ITestOutputHelper TestOutputHelper
) : IClassFixture<SqlServerFixture>
{
    // Override the default image tag by passing a custom one to the fixture constructor.
    // If no imageTag is provided, the default image tag will be used:
    // - SQL Server: mcr.microsoft.com/mssql/server:2022-CU21-ubuntu-22.04
    // - PostgreSQL: postgres:18.1
    
    public SomeTests()
    {
        SqlServerFixture = new SqlServerFixture("mcr.microsoft.com/mssql/server:2019-latest");
    }

    [Fact]
    public async Task Example_Test()
    {
        // Use the fixture with your custom image tag...
    }
}
```

### Using Entity Framework Core.
```csharp
public class SomeTests(
    SqlServerFixture SqlServerFixture, // The MSSql Testcontainer. Either this simple one or your own custom one.
    ITestOutputHelper TestOutputHelper // xUnit's output helper.
)
{
    private static readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task ToListAsync_GivenAnExistingDatabase_ShouldReturnAllUsers()
    {
        // Arrange.
        // Grab our unique connection string.
        var connectionString = SqlServerFixture.CreateDbConnectionString(TestContext.Current, TestOutputHelper);

        // ** We're using Entity Framework to connect to our unique Database instance.
        // ** Can you anything you like here - like Dapper, etc.

        // Wire up our EF Core DbContext to this unique connection string.
        var efOptions = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        // Create our context.
        var dbContext = new SqlServerDbContext(efOptions);

        // Create the dabase if it doesn't exist (which it shouldn't)
        await dbContext.Database.EnsureCreatedAsync(testContext.CancellationToken);

        // ** DB is ready to go.

        // Setup some fake data.
        var user = new User
        {
            FirstName = "Princess",
            LastName = "Leia",
            Email = "i.am.a.princess@example.com"
        };

        await dbContext.Users.AddAsync(user, _cancellationToken);
        await dbContext.SaveChangesAsync(_cancellationToken);

        // ** DB now has some data.

        // Act.
        var users = await dbContext.Users.ToListAsync(_cancellationToken);

        // Assert.
        Assert.NotNull(users);
    }
}
```

---

## Contribute
Yep - contributions are always welcome. Please read the contribution guidelines first.

## Code of Conduct

If you wish to participate in this repository then you need to abide by the code of conduct.

## Feedback

Yes! Please use the Issues section to provide feedback - either good or needs improvement :cool:

---
