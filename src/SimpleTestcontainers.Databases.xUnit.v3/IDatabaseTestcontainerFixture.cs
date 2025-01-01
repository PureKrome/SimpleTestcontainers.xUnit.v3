using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTestcontainers.Databases.xUnit.v3;

public interface IDatabaseTestcontainerFixture
{
    public string ConnectionString { get; }
    public Task InitializeAsync();
    public Task DisposeAsync();
}
