namespace OrdersDispatcher.Services;

using System.Text.Json;

using Microsoft.Extensions.Logging;

/// <summary>
/// A simple implementation of <see cref="IAggregatedOrderDispatcher"/> that simulates dispatching by aggregating incoming data and printing it as JSON.
/// </summary>
public class AggregatedOrderDispatcher : IAggregatedOrderDispatcher
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<AggregatedOrderDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatedOrderDispatcher"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public AggregatedOrderDispatcher(ILogger<AggregatedOrderDispatcher> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Dispatches the provided aggregates by serializing them to JSON and logging the output. In a real implementation, this method would send the data to an internal system or service.
    /// </summary>
    /// <param name="aggregates">The aggregated orders to dispatch.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    public Task DispatchAsync(IReadOnlyCollection<ProductOrderAggregate> aggregates, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(aggregates, SerializerOptions);
        _logger.LogInformation("Dispatching aggregated orders: {Json}", json);
        return Task.CompletedTask;
    }
}