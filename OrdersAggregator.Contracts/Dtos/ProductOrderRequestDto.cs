namespace OrdersAggregator.Contracts.Dtos;

/// <summary>
/// Represents a batch of one or more product orders submitted for aggregation.
/// </summary>
/// <param name="ProductOrders">The collection of product orders.</param>
public sealed record ProductOrderRequestDto(IEnumerable<ProductOrderDto> ProductOrders);
