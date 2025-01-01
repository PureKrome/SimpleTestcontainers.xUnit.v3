using Microsoft.Data.SqlClient;
using WorldDomination.SimpleTestcontainers.xUnit.v3.Databases;
using Xunit;

namespace WorldDomination.SimpleTestcontainers.xUnit.v3.MsSql;

public static class SqlServerTestHelpers
{
    /// <summary>
    /// Creates a connection string which will be for a unique DB that is based off the test name.
    /// </summary>
    /// <param name="sqlServerFixture">The custom Testcontainers SqlServer fixture</param>
    /// <param name="testContext">The current xUnit test contenxt. This contains the currently running test name.</param>
    /// <param name="testOutputHelper">Optional: The xUnit test output help where any logging can goto. If this is provided, then the connection string is writen to the test output in case you need to manually debug your data during the running of the test or afterwards is the image instance hasn't been destroyed (see <code>isReused</code> in <see cref="SqlServerFixture"/>).</param>
    /// <returns>The connection string with the unique database.</returns>
    public static UniqueConnectionStringInfo CreateUniqueConnectionStringInfo(
        this SqlServerFixture sqlServerFixture,
        ITestContext testContext,
        ITestOutputHelper? testOutputHelper = null) => sqlServerFixture.CreateUniqueConnectionStringInfo(
            testContext.Test!.TestDisplayName,
            testOutputHelper);

    /// <summary>
    /// Creates a connection string which will be for a unique DB that is based off the provided database name.
    /// </summary>
    /// <param name="sqlServerFixture">The custom Testcontainers SqlServer fixture</param>
    /// <param name="databaseName">Unique name of the database.</param>
    /// <param name="testOutputHelper">Optional: The xUnit test output help where any logging can goto. If this is provided, then the connection string is writen to the test output in case you need to manually debug your data during the running of the test or afterwards is the image instance hasn't been destroyed (see <code>isReused</code> in <see cref="SqlServerFixture"/>).</param>
    /// <remarks>The database name will always be postpended with a Guid. If the length of this unique name is greater than 100 characters, it will be truncated to 100 with the Guid portion always remaining.</remarks>
    /// <returns>The connection string with the unique database.</returns>
    public static UniqueConnectionStringInfo CreateUniqueConnectionStringInfo(
        this SqlServerFixture sqlServerFixture,
        string databaseName,
        ITestOutputHelper? testOutputHelper) =>
        DatabaseFixtureHelpers.CreateUniqueConnectionStringInfo(
            sqlServerFixture,
            databaseName,
            CreateUniqueConnectionString,
            testOutputHelper);

    private static string CreateUniqueConnectionString(
        string connectionString,
        string uniqueDatabaseName)
    {
        var uniqueConnectionString = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = uniqueDatabaseName
        }.ToString();

        return uniqueConnectionString;
    }
}
