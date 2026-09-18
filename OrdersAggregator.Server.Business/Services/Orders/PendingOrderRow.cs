namespace OrdersAggregator.Server.Business.Services.Orders;

/// <summary>
/// The columns of a pending order line that the dispatch pipeline needs.
/// </summary>
/// <remarks>Grouping happens in memory rather than in the query because the identifiers have to survive the
/// aggregation; without them there is no way to mark exactly the dispatched rows afterwards.</remarks>
/// <param name="Id">The order line identifier.</param>
/// <param name="ProductId">The product the order line refers to.</param>
/// <param name="Quantity">The ordered quantity.</param>
internal sealed record PendingOrderRow(Guid Id, string ProductId, long Quantity);
