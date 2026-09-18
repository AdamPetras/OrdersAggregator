namespace OrdersAggregator.Server.Business.Services.Orders;

using Microsoft.Extensions.Logging;

/// <summary>
/// Source-generated log messages emitted by <see cref="OrderService"/>.
/// </summary>
/// <remarks>Message templates live here rather than inline so that every field name reaching the log backend is
/// declared in one place. Named placeholders become queryable attributes in Loki, so they must stay stable.</remarks>
internal static partial class OrderServiceLogs
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Accepted {OrderCount} order line(s) across {ProductCount} product(s), total quantity {TotalQuantity}.")]
    public static partial void OrdersAccepted(ILogger logger, int orderCount, int productCount, long totalQuantity);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Received an empty order batch; nothing was persisted.")]
    public static partial void EmptyOrderBatch(ILogger logger);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Failed to persist {OrderCount} order line(s).")]
    public static partial void OrderPersistenceFailed(ILogger logger, int orderCount, Exception exception);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Debug,
        Message = "Read {ProductCount} pending product aggregate(s) awaiting dispatch.")]
    public static partial void PendingAggregatesRead(ILogger logger, int productCount);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "Marked {OrderCount} order line(s) as dispatched.")]
    public static partial void OrdersMarkedDispatched(ILogger logger, int orderCount);
}
