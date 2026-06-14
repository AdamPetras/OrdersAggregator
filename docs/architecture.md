# Architecture

## Runtime overview

OrdersAggregator is currently a hosted Blazor WebAssembly application:

```text
Browser
  -> Blazor WebAssembly client
  -> POST /api/orders on the ASP.NET Core host
  -> business service
  -> EF Core DbContext
  -> in-memory persistence

Background service
  -> reads pending rows every 20 seconds
  -> groups rows by ProductId
  -> logs aggregated JSON payloads
```

The server host is the central process. It serves static client assets, exposes the API controllers, enables OpenAPI in Development, ensures the EF database is created, and starts the background dispatcher.

## Project responsibilities

| Project | Details |
| --- | --- |
| `OrdersAggregator` | MudBlazor-based UI, Refit API client registration, and client-side error presentation. |
| `OrdersAggregator.Server` | ASP.NET Core composition root, middleware pipeline, configuration loading, controller endpoints, and hosted client assets. |
| `OrdersAggregator.Server.Business` | `IOrderService` and the current persistence/query logic for incoming orders. |
| `OrdersDispatcher` | Dispatch interval options, hosted background worker, and the current log-based dispatcher implementation. |
| `OrdersAggregator.DAL` | `OrdersDbContext`, provider registration, and startup database initialization helper. |
| `OrdersAggregator.DAL.Entities` | `ProductOrderEntity`, the persistence model for stored order lines. |
| `OrdersAggregator.Contracts` | Request/response DTOs and shared JSON serializer configuration. |
| `OrdersAggregator.Core` | Reusable options binding infrastructure. |

## Request and dispatch flow

### 1. Client submission

`OrdersAggregator\Pages\Home.razor` renders the only functional screen. It lets the user add and remove order lines, then constructs a `ProductOrderRequestDto` and sends it through `IOrdersApi`.

Important client-side characteristics:

- The client uses Refit and shared serializer settings from `OrdersAggregator.Contracts`.
- The `HttpClient` base address is the browser host origin, so the server-hosted setup is the intended way to run the full application.
- Quantity input is constrained in the UI, but the server currently does not mirror that validation in business logic.

### 2. API entry point

`OrdersAggregator.Server\Controllers\OrdersController.cs` exposes `POST /api/orders`.

Current controller behavior:

- Returns `400 Bad Request` when the request body is `null`.
- Delegates non-null bodies to `IOrderService`.
- Returns `202 Accepted` with the number of accepted order lines.
- Has a catch block for `OrderSubmissionValidationException`, although the current business implementation does not throw that exception.

### 3. Persistence

`OrderService` currently stores each incoming order line as a new `ProductOrderEntity` row:

- `ProductId`
- `Quantity`
- `DispatchedAt`

Pending work is queried with `DispatchedAt == null` and then grouped by `ProductId` when the dispatcher asks for the next batch.

### 4. Background dispatch

`OrdersDispatcher\Services\OrderDispatchBackgroundService.cs` uses `PeriodicTimer` with the configured dispatch interval. On each tick it:

1. Calls `IOrderService.TakeGroupedPendingAsync()`.
2. Skips the cycle if there are no pending aggregates.
3. Passes the grouped result to `IAggregatedOrderDispatcher`.

`AggregatedOrderDispatcher` is the current downstream stub. It serializes the aggregates and writes them to the server log.

## Configuration model

| Source | Purpose |
| --- | --- |
| `OrdersAggregator.Server\appsettings.json` | Base logging, `DbConnection`, and `OrderDispatch` settings. |
| `OrdersAggregator.Server\appsettings.Development.json` | Development CORS origins. |
| User secrets on `OrdersAggregator.Server` | Local server-side secrets when a real database or downstream integration is added. |

The reusable options plumbing lives in `OrdersAggregator.Core\Configuration`. Both `ConnectionStringOptions` and `OrderDispatchOptions` bind and validate through `OptionsConfigurationBase<T>`.

## Important implementation gaps

These gaps are worth knowing before building new features on top of the current code:

1. **Persistence provider selection is not finished.** `ConnectionStringOptions` validates PostgreSQL configuration, but `DalInstaller` currently hardcodes `UseInMemoryDatabase(...)`.
2. **Pending rows are never completed.** Dispatch reads rows where `DispatchedAt == null`, but no code updates or removes those rows after dispatch, so the same aggregates remain pending.
3. **The EF model and write path do not yet match a relational aggregate design.** `ProductOrderEntityConfiguration` declares a unique index on `ProductId`, while `OrderService` inserts one row per incoming line instead of updating an existing aggregate. The current in-memory provider does not surface that mismatch the way a relational provider would.
4. **Validation scaffolding is ahead of the implementation.** `OrderSubmissionValidationException` exists, but no business validation currently throws it.
5. **Automated tests are only scaffolded.** The test projects exist, but there are no discovered test cases yet.
