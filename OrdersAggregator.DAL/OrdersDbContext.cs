using Microsoft.EntityFrameworkCore;

namespace OrdersAggregator.DAL;

/// <summary>
/// Provides database access for order aggregation persistence.
/// </summary>
public class OrdersDbContext : OrdersDbContextBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersDbContext"/> class.
    /// </summary>
    /// <param name="options">The configured Entity Framework options.</param>
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }
}