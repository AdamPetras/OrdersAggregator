namespace OrdersAggregator.Contracts.Dtos;

/// <summary>
/// Represents the result of a successful product order submission, including the number of accepted orders and the number of distinct products represented by the batch.
/// </summary>
/// <param name="AcceptedOrders">The number of order lines accepted from the request body.</param>
public sealed record ProductOrderResponseDto(int AcceptedOrders);
