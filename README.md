# OrdersAggregator

OrdersAggregator is a .NET 10 sample application for collecting product order lines, storing them on the server, and periodically emitting aggregated quantities per product. The UI is a Blazor WebAssembly client, the backend is an ASP.NET Core host, persistence is handled with Entity Framework Core, and background dispatch currently writes aggregated JSON payloads to the server log.

## Current state

- `OrdersAggregator.Server` is the main local entry point. It serves the Blazor client and the API from the same origin.
- The DAL already contains PostgreSQL-oriented configuration types and packages, but the current `DalInstaller` always wires EF Core to the in-memory provider.
- The background dispatcher runs every 20 seconds and logs grouped pending orders. Pending rows are not marked as dispatched yet, so the same grouped payload can be emitted again on the next cycle.
- The solution contains xUnit test projects, but they currently do not contain discovered tests.

## Solution layout

| Project | Responsibility |
| --- | --- |
| `OrdersAggregator` | Blazor WebAssembly client for entering and submitting order lines. |
| `OrdersAggregator.Server` | ASP.NET Core host that serves the client, exposes the API, configures OpenAPI in Development, and starts infrastructure services. |
| `OrdersAggregator.Server.Business` | Order intake and query logic behind the API. |
| `OrdersDispatcher` | Background dispatch pipeline and dispatch interval configuration. |
| `OrdersAggregator.DAL` | Entity Framework Core registration, DbContext types, and startup helpers. |
| `OrdersAggregator.DAL.Entities` | Persistence model types. |
| `OrdersAggregator.Contracts` | Shared DTOs and shared JSON serializer settings for client/server communication. |
| `OrdersAggregator.Core` | Shared options/configuration abstractions. |
| `OrdersAggregator.Server.Business.Tests` | Scaffolded xUnit project for business-layer tests. |
| `OrdersAggregator.DAL.Tests` | Scaffolded xUnit project for DAL tests. |

## Getting started

### Prerequisites

- .NET 10 SDK

### Build

```powershell
dotnet build OrdersAggregator.slnx -c Debug
```

### Run locally

```powershell
dotnet run --project OrdersAggregator.Server\OrdersAggregator.Server.csproj
```

The default launch settings expose the hosted app at:

- `https://localhost:7286`
- `http://localhost:5250`

Use the server project for end-to-end local development. The standalone client project has its own launch profile, but the current client code sends API requests to its own origin, so the hosted server setup is the working development path.

## Order submission flow

1. The client builds a `ProductOrderRequestDto` from the form on `Pages/Home.razor`.
2. `IOrdersApi` sends the batch to `POST /api/orders`.
3. `OrdersController` forwards the request to `IOrderService`.
4. `OrderService` stores each submitted line in `OrdersDbContext`.
5. `OrderDispatchBackgroundService` wakes up every 20 seconds, groups pending rows by `ProductId`, and passes the aggregated payload to `IAggregatedOrderDispatcher`.
6. `AggregatedOrderDispatcher` serializes the grouped payload and logs it.

## Configuration

Most runtime configuration lives in `OrdersAggregator.Server\appsettings.json`:

- `DbConnection` contains the future database provider settings. At the moment, the code still uses the in-memory EF provider regardless of these values.
- `OrderDispatch:DispatchIntervalSeconds` controls the dispatcher cadence and must be at least `20`.

Development-only origins for cross-origin testing live in `OrdersAggregator.Server\appsettings.Development.json`. The server project also loads user secrets during startup, so server-side local secrets belong there.

## API summary

The single API endpoint is `POST /api/orders`. It accepts a JSON body shaped like:

```json
{
  "productOrders": [
    {
      "productId": "SKU-001",
      "quantity": 3,
      "dispatchedAt": null
    }
  ]
}
```

A successful submission returns `202 Accepted` with:

```json
{
  "acceptedOrders": 1
}
```

More detail is in [docs/api.md](docs/api.md).

## Additional documentation

- [docs/architecture.md](docs/architecture.md)
- [docs/development.md](docs/development.md)
- [docs/api.md](docs/api.md)
