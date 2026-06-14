namespace OrdersAggregator.Server.Business.Services.Orders;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrdersAggregator.Contracts.Dtos;
using OrdersAggregator.DAL;
using OrdersAggregator.DAL.Services;

/// <summary>
/// Validates incoming order lines and stores them as per-product aggregates.
/// </summary>
public sealed class OrderService : IOrderService
{
    private readonly IOrdersDbContextProvider _ordersDbContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="ordersDbContextProvider">The provider used to obtain instances of <see cref="OrdersDbContext"/> for database operations.</param>
    public OrderService(IOrdersDbContextProvider ordersDbContextProvider)
    {
        ArgumentNullException.ThrowIfNull(ordersDbContextProvider);
        _ordersDbContextProvider = ordersDbContextProvider;
    }

    /// <inheritdoc />
    public async Task<OrderSubmissionResult> AddOrdersAsync(
        IEnumerable<ProductOrderDto> orders,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orders);

        IReadOnlyList<ProductOrderDto> orderList = orders as IReadOnlyList<ProductOrderDto> ?? orders.ToArray();

        await using OrdersDbContextBase context = await _ordersDbContextProvider.GetDbContextAsync(cancellationToken);
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        await context.ProductOrderAggregates.AddRangeAsync(orderList.Select(OrderMapper.ToEntity), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new OrderSubmissionResult(orderList.Count);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ProductOrderAggregatedDto>> TakeGroupedPendingAsync(CancellationToken cancellationToken = default)
    {
        await using OrdersDbContextBase context = await _ordersDbContextProvider.GetDbContextAsync(cancellationToken);
        List<ProductOrderAggregatedDto> pendingOrders = await context.ProductOrderAggregates.AsNoTracking()
           .Where(x => x.DispatchedAt == null)
           .GroupBy(x => x.ProductId)
           .Select(x => new ProductOrderAggregatedDto(x.Key, x.Sum(y => y.Quantity)))
           .ToListAsync(cancellationToken: cancellationToken);
        return pendingOrders;
    }
}