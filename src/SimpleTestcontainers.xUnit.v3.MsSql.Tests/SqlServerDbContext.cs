using Microsoft.EntityFrameworkCore;

namespace SimpleTestcontainers.xUnit.v3.MsSql.Tests;

public class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
