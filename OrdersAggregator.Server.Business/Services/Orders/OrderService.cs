namespace OrdersAggregator.Server.Business.Services.Orders;

using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrdersAggregator.Contracts.Dtos;
using OrdersAggregator.Core.Diagnostics;
using OrdersAggregator.DAL;
using OrdersAggregator.DAL.Entities;
using OrdersAggregator.DAL.Services;

/// <summary>
/// Validates incoming order lines and stores them as per-product aggregates.
/// </summary>
public sealed class OrderService : IOrderService
{
    private const string SubmitActivityName = "orders.submit";
    private const string ReadPendingActivityName = "orders.read_pending";
    private const string MarkDispatchedActivityName = "orders.mark_dispatched";

    private readonly IOrdersDbContextProvider _ordersDbContextProvider;
    private readonly OrdersMetrics _metrics;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<OrderService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="ordersDbContextProvider">The provider used to obtain instances of <see cref="OrdersDbContext"/> for database operations.</param>
    /// <param name="metrics">The instrumentation used to record order lifecycle measurements.</param>
    /// <param name="timeProvider">The clock used to stamp dispatch times.</param>
    /// <param name="logger">The logger used to record order intake activity.</param>
    public OrderService(
        IOrdersDbContextProvider ordersDbContextProvider,
        OrdersMetrics metrics,
        TimeProvider timeProvider,
        ILogger<OrderService> logger)
    {
        ArgumentNullException.ThrowIfNull(ordersDbContextProvider);
        ArgumentNullException.ThrowIfNull(metrics);
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(logger);

        _ordersDbContextProvider = ordersDbContextProvider;
        _metrics = metrics;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<OrderSubmissionResult> AddOrdersAsync(
        IEnumerable<ProductOrderDto> orders,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orders);

        IReadOnlyList<ProductOrderDto> orderList = orders as IReadOnlyList<ProductOrderDto> ?? orders.ToArray();

        using Activity? activity = OrdersTelemetry.ActivitySource.StartActivity(SubmitActivityName);
        activity?.SetTag("orders.count", orderList.Count);

        if (orderList.Count == 0)
        {
            OrderServiceLogs.EmptyOrderBatch(_logger);
            return new OrderSubmissionResult(0);
        }

        long totalQuantity = orderList.Sum(order => order.Quantity);
        int productCount = orderList.Select(order => order.ProductId).Distinct(StringComparer.Ordinal).Count();

        activity?.SetTag("orders.product_count", productCount);
        activity?.SetTag("orders.total_quantity", totalQuantity);

        try
        {
            await using OrdersDbContextBase context = await _ordersDbContextProvider.GetDbContextAsync(cancellationToken);

            await context.ProductOrderAggregates.AddRangeAsync(orderList.Select(OrderMapper.ToEntity), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            OrderServiceLogs.OrderPersistenceFailed(_logger, orderList.Count, exception);
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }

        _metrics.RecordSubmission(orderList.Count, totalQuantity);
        OrderServiceLogs.OrdersAccepted(_logger, orderList.Count, productCount, totalQuantity);

        return new OrderSubmissionResult(orderList.Count);
    }

    /// <inheritdoc />
    public async Task<PendingOrderBatch> GetGroupedPendingAsync(CancellationToken cancellationToken = default)
    {
        using Activity? activity = OrdersTelemetry.ActivitySource.StartActivity(ReadPendingActivityName);

        await using OrdersDbContextBase context = await _ordersDbContextProvider.GetDbContextAsync(cancellationToken);

        List<PendingOrderRow> pendingRows = await context.ProductOrderAggregates.AsNoTracking()
           .Where(x => x.DispatchedAt == null)
           .Select(x => new PendingOrderRow(x.Id, x.ProductId, x.Quantity))
           .ToListAsync(cancellationToken: cancellationToken);

        if (pendingRows.Count == 0)
        {
            activity?.SetTag("orders.product_count", 0);
            OrderServiceLogs.PendingAggregatesRead(_logger, 0);
            return PendingOrderBatch.Empty;
        }

        ProductOrderAggregatedDto[] aggregates = [.. pendingRows
            .GroupBy(row => row.ProductId, StringComparer.Ordinal)
            .Select(group => new ProductOrderAggregatedDto(group.Key, group.Sum(row => row.Quantity)))];

        Guid[] orderIds = [.. pendingRows.Select(row => row.Id)];

        activity?.SetTag("orders.product_count", aggregates.Length);
        activity?.SetTag("orders.count", orderIds.Length);
        OrderServiceLogs.PendingAggregatesRead(_logger, aggregates.Length);

        return new PendingOrderBatch(aggregates, orderIds);
    }

    /// <inheritdoc />
    public async Task<int> MarkDispatchedAsync(
        IReadOnlyCollection<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orderIds);

        if (orderIds.Count == 0)
        {
            return 0;
        }

        using Activity? activity = OrdersTelemetry.ActivitySource.StartActivity(MarkDispatchedActivityName);
        activity?.SetTag("orders.count", orderIds.Count);

        await using OrdersDbContextBase context = await _ordersDbContextProvider.GetDbContextAsync(cancellationToken);

        // Tracked on purpose: the in-memory provider does not support ExecuteUpdateAsync, so the rows are
        // loaded and saved rather than updated in a single statement.
        List<ProductOrderEntity> rows = await context.ProductOrderAggregates
           .Where(entity => orderIds.Contains(entity.Id) && entity.DispatchedAt == null)
           .ToListAsync(cancellationToken: cancellationToken);

        if (rows.Count == 0)
        {
            return 0;
        }

        DateTimeOffset dispatchedAt = _timeProvider.GetUtcNow();
        foreach (ProductOrderEntity row in rows)
        {
            row.DispatchedAt = dispatchedAt;
        }

        await context.SaveChangesAsync(cancellationToken);

        OrderServiceLogs.OrdersMarkedDispatched(_logger, rows.Count);

        return rows.Count;
    }
}
