# API

## Endpoint summary

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/orders` | Accept a batch of product order lines for asynchronous processing. |

## Request contract

The request body is a `ProductOrderRequestDto`:

```json
{
  "productOrders": [
    {
      "productId": "SKU-001",
      "quantity": 3,
      "dispatchedAt": null
    },
    {
      "productId": "SKU-002",
      "quantity": 5,
      "dispatchedAt": null
    }
  ]
}
```

Field notes:

- `productOrders` is the collection of submitted lines.
- `productId` is the external product identifier used for grouping.
- `quantity` is stored as a `long`.
- `dispatchedAt` exists on the shared DTO and entity. The shipped UI always sends `null`, and the current dispatch flow treats `null` rows as pending.

## Success response

Successful submissions return `202 Accepted` with a `ProductOrderResponseDto`:

```json
{
  "acceptedOrders": 2
}
```

`acceptedOrders` is the number of order lines received by the server, not the number of distinct products after grouping.

## Error behavior

Current application-level behavior is minimal:

- The controller explicitly returns `400 Bad Request` when the request body is `null`.
- `OrderSubmissionValidationException` is wired for `400 Bad Request` responses, but the current business implementation does not throw it.
- Malformed JSON or model-binding failures still follow normal ASP.NET Core API behavior.

## Processing semantics

The current implementation is important for integrators to understand:

1. Accepted requests are stored immediately in the server-side EF Core database.
2. Aggregation happens later, when the background worker queries pending rows and groups them by `ProductId`.
3. The dispatcher runs every `OrderDispatch:DispatchIntervalSeconds` seconds, with a minimum supported value of `20`.
4. The current dispatcher only logs the grouped payload.
5. Rows are not marked as dispatched after logging, so the same pending aggregates can be emitted repeatedly on later cycles.

In other words, `202 Accepted` currently means "stored for background processing", not "delivered exactly once to a downstream system."
