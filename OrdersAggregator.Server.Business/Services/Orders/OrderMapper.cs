namespace OrdersAggregator.Server.Business.Services.Orders;

using OrdersAggregator.Contracts.Dtos;
using OrdersAggregator.DAL.Entities;

/// <summary>
/// Provides mapping between order-related data transfer objects (DTOs) and data access layer (DAL) entities.
/// </summary>
public class OrderMapper
{
    /// <summary>
    /// Maps a <see cref="ProductOrderDto"/> to a <see cref="ProductOrderEntity"/>.
    /// </summary>
    /// <param name="dto">The product order DTO to map.</param>
    /// <returns>The mapped product order entity.</returns>
    public static ProductOrderEntity ToEntity(ProductOrderDto dto)
    {
        return new ProductOrderEntity()
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                DispatchedAt = dto.DispatchedAt,
            };
    }
}