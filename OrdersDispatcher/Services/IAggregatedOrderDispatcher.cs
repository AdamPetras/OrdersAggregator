namespace OrdersDispatcher.Services;

using OrdersAggregator.Contracts.Dtos;

/// <summary>
/// Sends aggregated product orders to the downstream internal system.
/// </summary>
public interface IAggregatedOrderDispatcher
{
    /// <summary>
    /// Dispatches the provided aggregates to the internal system.
    /// </summary>
    /// <param name="aggregates">The aggregated orders to dispatch.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    Task DispatchAsync(IReadOnlyCollection<ProductOrderAggregatedDto> aggregates, CancellationToken cancellationToken = default);
}