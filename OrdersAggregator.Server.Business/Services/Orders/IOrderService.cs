namespace OrdersAggregator.Server.Business.Services.Orders
{
    using OrdersAggregator.Contracts.Dtos;

    /// <summary>
    /// Accepts incoming order batches and persists their aggregated state for later dispatch.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Validates, aggregates, and stores the submitted orders for asynchronous dispatch.
        /// </summary>
        /// <param name="orders">The submitted order lines.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>A summary of the accepted order batch.</returns>
        Task<OrderSubmissionResult> AddOrdersAsync(
            IEnumerable<ProductOrderDto> orders,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all pending orders that have not yet been dispatched, grouped by product.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>A collection of aggregated product orders.</returns>
        Task<IReadOnlyCollection<ProductOrderAggregatedDto>> TakeGroupedPendingAsync(CancellationToken cancellationToken = default);
    }
}
