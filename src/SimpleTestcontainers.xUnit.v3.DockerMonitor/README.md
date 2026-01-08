# Docker Container Monitor

A console application that monitors Docker containers used by the SimpleTestcontainers library and displays their database information with historical tracking.

<p align="center">
  <img alt="example output" src="https://github.com/user-attachments/assets/9c1f18c6-eeb6-4bd5-984b-46f9ac757289" />
</p>

## Features

- Monitors running Docker containers every 0.5 seconds
- Filters containers by name patterns:
  - `test-container`
  - `SqlServer-Tests`
  - `PostgreSql-Tests`
- Displays container information (name, image, status, ports)
- **Remembers containers and databases even after they stop or are removed**
- Tracks database state (Active/Gone) over time
- For SQL Server containers: Lists all user databases with status
- For PostgreSQL containers: Lists all user databases with status
- Uniquely identifies containers by Docker ID to track multiple instances with the same name

## Prerequisites

- .NET 10 SDK
- Docker Desktop running
- Running testcontainers (from your xUnit tests)

## How to Run

### Option 1: Using .NET CLI

```bash
cd src/SimpleTestcontainers.xUnit.v3.DockerMonitor
dotnet run
```

### Option 2: Build and Run

```bash
cd src/SimpleTestcontainers.xUnit.v3.DockerMonitor
dotnet build
dotnet run --no-build
```

### Option 3: Run from Solution Root

```bash
dotnet run --project src/SimpleTestcontainers.xUnit.v3.DockerMonitor/SimpleTestcontainers.xUnit.v3.DockerMonitor.csproj
```

## What It Does

The monitor will:

1. Connect to your local Docker instance
2. List all running containers matching the filter patterns
3. **Remember all containers it has ever seen** (even after they stop)
4. For each container, display:
   - Container name
   - Container ID (shortened to 12 characters)
   - Active/Inactive status
   - Image name
   - For active containers: Status and port mappings
   - For inactive containers: Last seen timestamp
5. For SQL Server containers (`SqlServer-Tests`):
   - Connect using default testcontainers credentials
   - List all user databases (excluding system databases)
   - Track database state: **Active** (currently exists) or **Gone** (previously existed)
6. For PostgreSQL containers (`PostgreSql-Tests`):
   - Connect using default testcontainers credentials
   - List all user databases (excluding template and postgres databases)
   - Track database state: **Active** (currently exists) or **Gone** (previously existed)

## State Tracking

The monitor maintains historical state across container lifecycles:

- **Containers**: Each container is uniquely identified by its Docker ID, allowing multiple containers with the same name to be tracked separately
- **Databases**: Once discovered, databases are remembered even if the container is removed
- **Active/Inactive Status**: 
  - Containers show as **ACTIVE** when running, **INACTIVE** when stopped/removed
  - Databases show as **Active** when they exist, **Gone** when deleted or container is stopped
- **Timestamps**: Tracks first seen and last seen times for both containers and databases

## Example Output

```
Last Update: 2024-01-15 14:30:45

Container: SqlServer-Tests-10.0.0 / ID: 1a2b3c4d5e6f / [ACTIVE]
  Image: mcr.microsoft.com/mssql/server:2022-CU21-ubuntu-22.04
  Status: Up 5 minutes
  Ports: 55001:1433
  Databases (Active: 3, Inactive: 0):
    - integrationtests1_abc123def456 (Active)
    - integrationtests2_def456abc789 (Active)
    - integrationtests3_ghi789jkl012 (Active)

Container: SqlServer-Tests-10.0.0 / ID: 7g8h9i0j1k2l / [INACTIVE]
  Image: mcr.microsoft.com/mssql/server:2022-CU21-ubuntu-22.04
  Last Seen: 2024-01-15 14:25:30
  Databases (Active: 0, Inactive: 2):
    - integrationtests_old1_xyz123 (Gone)
    - integrationtests_old2_mno456 (Gone)

Container: PostgreSql-Tests-10.0.0 / ID: 3m4n5o6p7q8r / [ACTIVE]
  Image: postgres:18.1
  Status: Up 3 minutes
  Ports: 55002:5432
  Databases (Active: 2, Inactive: 1):
    - integrationtests_xyz123abc456 (Active)
    - integrationtests_mno789pqr012 (Active)
    - integrationtests_deleted_stu345 (Gone)
```

## How State Tracking Works

### Container Lifecycle
1. **First Detection**: Container is added to history with ID, name, image, and "First Seen" timestamp
2. **While Running**: Marked as **ACTIVE**, status and ports updated each loop
3. **After Stopping**: Marked as **INACTIVE**, displays "Last Seen" timestamp
4. **Multiple Instances**: Containers are tracked by unique Docker ID, so restarting a container with the same name creates a new entry

### Database Lifecycle
1. **First Detection**: Database added to container's list with "First Seen" timestamp
2. **While Exists**: Marked as **Active**, "Last Seen" updated each loop
3. **After Deletion**: Marked as **Gone** but remains in the list for historical reference
4. **Reappearing**: If a database with the same name appears again, it's marked **Active** again

## Default Credentials

The monitor uses the default credentials that testcontainers uses:

### SQL Server
- Username: `sa`
- Password: `yourStrong(!)Password`

### PostgreSQL
- Username: `postgres`
- Password: `postgres`

## Troubleshooting

### "Could not find port mapping"
- Ensure the containers are running and properly mapped to host ports
- Check Docker Desktop to verify container port mappings

### "Error connecting"
- Verify the containers are fully started and healthy
- Check that the default testcontainers credentials haven't been changed
- Ensure no firewall is blocking localhost connections
- Note: Connection errors are silently handled - the monitor will retry on the next loop

### No containers found
- Run your xUnit tests to start the testcontainers
- Verify container names match the filter patterns

### Databases not appearing
- Wait a few seconds after container starts - databases may not be immediately available
- Check container logs to ensure database service is fully started

## Press Ctrl+C to Exit

The monitor runs in an infinite loop. Press `Ctrl+C` to stop it. Historical data is lost when the monitor exits.
