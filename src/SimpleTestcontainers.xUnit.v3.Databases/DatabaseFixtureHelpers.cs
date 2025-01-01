using System.Text.RegularExpressions;
using Xunit;

namespace WorldDomination.SimpleTestcontainers.xUnit.v3.Databases;

public static class DatabaseFixtureHelpers
{
    /// <summary>
    /// Creates a connection string which will be for a unique DB that is based off the provided database name.
    /// </summary>
    /// <param name="databaseFixture">The custom Testcontainers SqlServer fixture</param>
    /// <param name="databaseName">Unique name of the database.</param>
    /// <param name="testOutputHelper">Optional: The xUnit test output help where any logging can goto. If this is provided, then the connection string is writen to the test output in case you need to manually debug your data during the running of the test or afterwards is the image instance hasn't been destroyed (see <code>isReused</code> in <see cref="SqlServerFixture"/>).</param>
    /// <remarks>The database name will always be postpended with a Guid. If the length of this unique name is greater than 100 characters, it will be truncated to 100 with the Guid portion always remaining.</remarks>
    /// <returns>The connection string with the unique database.</returns>
    public static UniqueConnectionStringInfo CreateUniqueConnectionStringInfo(
        this IDatabaseFixture databaseFixture,
        string databaseName,
        Func<string, string, string> createUniqueConnectionString,
        ITestOutputHelper? testOutputHelper)
    {
        // Generate a unique database name using the test class name and the test name.
        const int maxDatabaseNameLength = 100;// MSSql has a problem with long names - so we need to limit it. Max size is 125 (I think).
        var guid = Guid.NewGuid().ToString("N");
        var uniqueDbName = $"{databaseName}{guid}";

        // If the unique database name is too long, we need to trim it down.
        // We cannot rely on the provided database name being short enough.
        if (uniqueDbName.Length > maxDatabaseNameLength)
        {
            // We trim off the provided database name to make room for the guid, to a max length of 'maxDatabaseNameLength'.
            var truncatedText = uniqueDbName[..(maxDatabaseNameLength - guid.Length)];
            uniqueDbName = $"{truncatedText}_{guid}";
        }

        // Finally, remove bad characters that might cause issues.
        var cleanedDbName = uniqueDbName.ToLower();
        if (Regex.IsMatch(cleanedDbName, @"[^a-z0-9_]"))
        {
            cleanedDbName = Regex.Replace(cleanedDbName, @"[^a-z0-9_]", "_");
        }

        // Update the connection string to use the unique database name.
        // ⭐⭐ This is the magic! 
        var connectionString = createUniqueConnectionString(databaseFixture.ConnectionString, cleanedDbName);

        testOutputHelper?.WriteLine($"** Sql Server Connection String: {connectionString}");

        return new UniqueConnectionStringInfo(cleanedDbName, connectionString);
    }
}
