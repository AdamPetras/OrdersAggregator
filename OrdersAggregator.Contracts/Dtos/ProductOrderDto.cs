namespace OrdersAggregator.Contracts.Dtos;

/// <summary>
/// Represents a single product order received by the API.
/// </summary>
/// <param name="ProductId">The external product identifier.</param>
/// <param name="Quantity">The ordered quantity.</param>
/// <param name="DispatchedAt">The timestamp when the order was dispatched, if available.</param>
public record ProductOrderDto(string ProductId, long Quantity, DateTimeOffset? DispatchedAt);