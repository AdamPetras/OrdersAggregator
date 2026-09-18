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
        /// Retrieves all order lines that have not yet been dispatched, grouped by product.
        /// </summary>
        /// <remarks>This does not claim the rows. Call <see cref="MarkDispatchedAsync"/> with the returned
        /// identifiers once the payload has actually been dispatched.</remarks>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The pending aggregates and the order line identifiers they were computed from.</returns>
        Task<PendingOrderBatch> GetGroupedPendingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stamps the given order lines as dispatched so that later cycles no longer pick them up.
        /// </summary>
        /// <param name="orderIds">The identifiers of the order lines to mark.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The number of order lines that were marked.</returns>
        Task<int> MarkDispatchedAsync(
            IReadOnlyCollection<Guid> orderIds,
            CancellationToken cancellationToken = default);
    }
}
