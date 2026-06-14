namespace OrdersDispatcher.Services
{
    /// <summary>
    /// Represents a pending aggregated quantity for a single product.
    /// </summary>
    /// <param name="ProductId">The external product identifier.</param>
    /// <param name="Quantity">The total aggregated quantity for the product.</param>
    public sealed record ProductOrderAggregate(string ProductId, long Quantity);
}
