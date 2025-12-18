# Reusable GitHub Actions

This directory contains composite actions that can be reused across multiple workflows.

## Available Actions

### 1. pull-and-cache-docker-images

**Purpose:** Extracts Docker image versions from Testcontainers packages, pulls them, and caches them for faster subsequent runs.

**Usage:**
```yaml
- name: Pull and cache Docker images
  id: pull-images
  uses: ./.github/actions/pull-and-cache-docker-images
```

**Outputs:**
- `mssql-image`: The SQL Server Docker image tag (e.g., `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`)
- `postgres-image`: The PostgreSQL Docker image tag (e.g., `postgres:15.1`)

**What it does:**
1. Runs the `.NET 10 file-based app` to extract actual image versions
2. Sets up Docker Buildx
3. Checks if images are cached
4. Pulls both Docker images in parallel (if not cached)
5. Saves images to `/tmp/.docker-cache/` as tar files
6. Outputs the image tags for downstream jobs

**Cache key format:**
```
docker-images-{OS}-{MSSQL_IMAGE}-{POSTGRES_IMAGE}
```

---

### 2. load-cached-docker-image

**Purpose:** Loads a previously cached Docker image for use in tests.

**Usage:**
```yaml
- name: Load cached Docker image
  uses: ./.github/actions/load-cached-docker-image
  with:
    cache-file: mssql.tar  # or postgres.tar
    mssql-image: ${{ needs.pull_docker_images.outputs.mssql-image }}
    postgres-image: ${{ needs.pull_docker_images.outputs.postgres-image }}
```

**Inputs:**
- `cache-file` (required): The tar file name in the cache (e.g., `mssql.tar`, `postgres.tar`)
- `mssql-image` (required): The SQL Server Docker image tag from the pull job
- `postgres-image` (required): The PostgreSQL Docker image tag from the pull job

**What it does:**
1. Restores the Docker image cache using the same key as the pull job
2. Loads the specified tar file into Docker
3. Falls back gracefully if cache is missing (Testcontainers will pull)

---

## Workflow Integration

### Example: Pull Request Workflow

```yaml
jobs:
  pull_docker_images:
    name: 🐳 Pull Docker images
    runs-on: ubuntu-latest
    outputs:
      mssql-image: ${{ steps.pull-images.outputs.mssql-image }}
      postgres-image: ${{ steps.pull-images.outputs.postgres-image }}
    steps:
      - uses: actions/checkout@v6
      - uses: actions/setup-dotnet@v5
      - name: Pull and cache Docker images
        id: pull-images
        uses: ./.github/actions/pull-and-cache-docker-images

  test:
    needs: pull_docker_images
    strategy:
      matrix:
        target:
          - name: MsSql
            cache-file: mssql.tar
          - name: PostgreSQL
            cache-file: postgres.tar
    steps:
      - uses: actions/checkout@v6
      - name: Load cached Docker image
        uses: ./.github/actions/load-cached-docker-image
        with:
          cache-file: ${{ matrix.target.cache-file }}
          mssql-image: ${{ needs.pull_docker_images.outputs.mssql-image }}
          postgres-image: ${{ needs.pull_docker_images.outputs.postgres-image }}
      # ... rest of test steps
```

---

## Benefits of Reusable Actions

✅ **DRY Principle** - Define once, use everywhere  
✅ **Consistency** - Same logic across all workflows  
✅ **Maintainability** - Update in one place, applies to all workflows  
✅ **Testability** - Actions can be tested independently  
✅ **Versioning** - Can version actions separately if needed  

---

## File Structure

```
.github/
├── actions/
│   ├── pull-and-cache-docker-images/
│   │   └── action.yml
│   └── load-cached-docker-image/
│       └── action.yml
├── scripts/
│   └── GetTestContainerDockerImageTags.cs
└── workflows/
    ├── PullRequest.yml (uses both actions)
    └── MergeToMain.yml (uses both actions)
```

---

## Maintenance

When updating Docker image handling logic:
1. Update the action file in `.github/actions/*/action.yml`
2. All workflows using the action automatically get the update
3. No need to modify multiple workflow files

---

## Technical Details

### Composite Actions
These are **composite actions** (as opposed to JavaScript or Docker actions), which means:
- They run shell commands and other actions
- They execute in the same runner environment as the workflow
- They have access to the repository files
- They're lightweight and fast

### Cache Strategy
Both actions use the same cache key format to ensure:
- The pull job creates the cache
- The test jobs restore the same cache
- Cache invalidates when image versions change
- Cache is shared across jobs in the same workflow run

---

## Workflows Using These Actions

| Workflow | Pull Images | Load Images | Purpose |
|----------|-------------|-------------|---------|
| `PullRequest.yml` | ✅ | ✅ | PR validation with cached images |
| `MergeToMain.yml` | ✅ | ✅ | Main branch testing with cached images |

Both workflows benefit from the same optimized Docker image handling! 🚀
