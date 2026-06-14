# Development guide

## Tooling

- .NET 10 SDK
- Optional: Docker, if you want to build the server container image from `OrdersAggregator.Server\Dockerfile`

## Common commands

### Build and analyzer run

```powershell
dotnet build OrdersAggregator.slnx -c Debug
```

`Directory.Build.props` enables analyzers during build and treats warnings as errors, so this is the repository's effective analyzer command.

### Tests

```powershell
dotnet test OrdersAggregator.slnx -c Debug
dotnet test OrdersAggregator.Server.Business.Tests\OrdersAggregator.Server.Business.Tests.csproj -c Debug
dotnet test OrdersAggregator.Server.Business.Tests\OrdersAggregator.Server.Business.Tests.csproj -c Debug --filter "FullyQualifiedName~Namespace.ClassName.MethodName"
```

The test projects currently build, but they do not contain discovered tests yet.

### Run the hosted app

```powershell
dotnet run --project OrdersAggregator.Server\OrdersAggregator.Server.csproj
```

Default local URLs from launch settings:

- `https://localhost:7286`
- `http://localhost:5250`

## Configuration reference

| Location | Notes |
| --- | --- |
| `OrdersAggregator.Server\appsettings.json` | Base logging, `DbConnection`, and `OrderDispatch` configuration. |
| `OrdersAggregator.Server\appsettings.Development.json` | CORS origins used for development scenarios. |
| `OrdersAggregator.Server` user secrets | Loaded at startup by the server project. Put server-side local secrets here. |
| `OrdersAggregator\Properties\launchSettings.json` | Standalone client launch profile. |
| `OrdersAggregator.Server\Properties\launchSettings.json` | Preferred end-to-end local launch profile. |

### Important note about the client

The client registers `HttpClient` and Refit clients with the browser host base address. That means the hosted server setup is the working local path for API calls. Running the client project by itself is useful only if you deliberately add a matching backend/proxy arrangement.

## Coding and repository conventions

### Package and build settings

- NuGet package versions are managed centrally in `Directory.Packages.props`.
- Repository-wide build defaults are defined in `Directory.Build.props`.
- All projects target `net10.0`, have nullable reference types enabled, generate XML documentation files, and treat warnings as errors.

### Time handling

`BannedSymbols.txt` blocks:

- `DateTime.Now`
- `DateTime.UtcNow`
- `DateTimeOffset.Now`
- `DateTimeOffset.UtcNow`

Use `TimeProvider` APIs instead.

### Formatting and style

Key `.editorconfig` choices:

- UTF-8 with BOM
- CRLF line endings
- 4-space indentation
- explicit types instead of `var`
- `_camelCase` private field naming

### StyleCop

`stylecop.json` relaxes several documentation rules. XML headers are disabled, and private/internal/public element documentation is not enforced globally even though the repository already contains XML comments in many files.

## Current implementation constraints

Keep these in mind while extending the codebase:

1. The DAL is currently wired to EF Core's in-memory provider even though PostgreSQL support is partially scaffolded.
2. The dispatcher logs aggregated orders but does not acknowledge, delete, or mark rows as dispatched.
3. `OrderSubmissionValidationException` is present as an API/business contract, but validation rules are not implemented yet.
4. The test projects are scaffolds and need real coverage to protect behavior changes.
