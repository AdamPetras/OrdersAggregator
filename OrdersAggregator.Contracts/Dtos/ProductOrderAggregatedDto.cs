namespace OrdersAggregator.Contracts.Dtos;

/// <summary>
/// Represents an aggregated product order, containing the product identifier and the total quantity ordered for that product.
/// </summary>
/// <param name="ProductId">The external product identifier.</param>
/// <param name="Quantity">The total quantity ordered for the product.</param>
public record ProductOrderAggregatedDto(string ProductId, long Quantity);