using Microsoft.EntityFrameworkCore;
using OrdersAggregator.DAL.Entities;

namespace OrdersAggregator.DAL
{
    /// <summary>
    /// Provides database access for order aggregation persistence.
    /// </summary>
    public sealed class OrdersDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersDbContext"/> class.
        /// </summary>
        /// <param name="options">The options used to configure the context.</param>
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
            : base(options)
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
}
