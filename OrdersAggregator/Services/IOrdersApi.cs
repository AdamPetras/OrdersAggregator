using OrdersAggregator.Contracts.Dtos;
using Refit;

namespace OrdersAggregator.Client.Services
{
    /// <summary>
    /// Defines the Refit client used by the Blazor application to submit product orders to the server API.
    /// </summary>
    public interface IOrdersApi
    {
        /// <summary>
        /// Sends a batch of product orders to the server for asynchronous aggregation and dispatch.
        /// </summary>
        /// <param name="request">The order batch to submit.</param>
        /// <param name="cancellationToken">The token used to cancel the HTTP request.</param>
        /// <returns>The accepted order count returned by the server.</returns>
        [Post("/api/orders")]
        Task<ProductOrderResponseDto> SubmitOrdersAsync(
            [Body] ProductOrderRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
