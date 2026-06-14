namespace OrdersAggregator.DAL;

using Microsoft.EntityFrameworkCore;

using OrdersAggregator.DAL.Entities;

/// <summary>
/// Provides database access for order aggregation persistence.
/// </summary>
public class OrdersDbContextBase : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersDbContextBase"/> class.
    /// </summary>
    public OrdersDbContextBase()
    {
    }

    /// <summary>
    /// Gets the per-product pending aggregate state rows.
    /// </summary>
    public DbSet<ProductOrderEntity> ProductOrderAggregates => this.Set<ProductOrderEntity>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
    }
}