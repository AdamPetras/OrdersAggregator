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
        /// Retrieves the pending (not yet dispatched) aggregated orders for processing by the dispatching mechanism.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The pending aggregated orders.</returns>
        Task<IReadOnlyCollection<ProductOrderDto>> TakePendingAsync(CancellationToken cancellationToken = default);
    }
}
