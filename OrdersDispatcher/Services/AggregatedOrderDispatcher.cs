namespace OrdersDispatcher.Services;

using System.Diagnostics;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using OrdersAggregator.Contracts.Dtos;
using OrdersAggregator.Contracts.Serialization;
using OrdersAggregator.Core.Diagnostics;

/// <summary>
/// A simple implementation of <see cref="IAggregatedOrderDispatcher"/> that simulates dispatching by aggregating incoming data and printing it as JSON.
/// </summary>
public class AggregatedOrderDispatcher : IAggregatedOrderDispatcher
{
    private const string DispatchActivityName = "orders.dispatch";

    private static readonly JsonSerializerOptions SerializerOptions = OrderApiJsonSerializer.Create();

    private readonly ILogger<AggregatedOrderDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatedOrderDispatcher"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public AggregatedOrderDispatcher(ILogger<AggregatedOrderDispatcher> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    /// <summary>
    /// Dispatches the provided aggregates by serializing them to JSON and logging the output. In a real implementation, this method would send the data to an internal system or service.
    /// </summary>
    /// <param name="aggregates">The aggregated orders to dispatch.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    public Task DispatchAsync(IReadOnlyCollection<ProductOrderAggregatedDto> aggregates, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(aggregates);

        using Activity? activity = OrdersTelemetry.ActivitySource.StartActivity(DispatchActivityName, ActivityKind.Producer);

        long totalQuantity = aggregates.Sum(aggregate => aggregate.Quantity);
        activity?.SetTag("orders.product_count", aggregates.Count);
        activity?.SetTag("orders.total_quantity", totalQuantity);

        string json = JsonSerializer.Serialize(aggregates, SerializerOptions);
        DispatcherLogs.AggregatedOrdersDispatched(_logger, aggregates.Count, totalQuantity, json);

        return Task.CompletedTask;
    }
}
