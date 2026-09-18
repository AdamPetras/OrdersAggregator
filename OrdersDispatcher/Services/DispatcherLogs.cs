namespace OrdersDispatcher.Services;

using Microsoft.Extensions.Logging;

/// <summary>
/// Source-generated log messages emitted by the dispatch pipeline.
/// </summary>
/// <remarks>Message templates live here rather than inline so that every field name reaching the log backend is
/// declared in one place. Named placeholders become queryable attributes in Loki, so they must stay stable.</remarks>
internal static partial class DispatcherLogs
{
    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "Order dispatch background service started with a {DispatchIntervalSeconds}s interval.")]
    public static partial void DispatchServiceStarted(ILogger logger, double dispatchIntervalSeconds);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Order dispatch background service is stopping.")]
    public static partial void DispatchServiceStopping(ILogger logger);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Debug,
        Message = "Dispatch cycle found no pending aggregates.")]
    public static partial void DispatchCycleEmpty(ILogger logger);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Error,
        Message = "Dispatch cycle failed; the service will retry on the next tick.")]
    public static partial void DispatchCycleFailed(ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 2100,
        Level = LogLevel.Information,
        Message = "Dispatching {ProductCount} aggregated product order(s), total quantity {TotalQuantity}: {AggregatedOrders}")]
    public static partial void AggregatedOrdersDispatched(
        ILogger logger,
        int productCount,
        long totalQuantity,
        string aggregatedOrders);
}
