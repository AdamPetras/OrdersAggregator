using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrdersAggregator.DAL.Entities;

namespace OrdersAggregator.DAL.Configurations
{
    /// <summary>
    /// Provides Entity Framework Core configuration for the <see cref="ProductOrderEntity"/> entity.
    /// </summary>
    internal sealed class ProductOrderEntityConfiguration : IEntityTypeConfiguration<ProductOrderEntity>
    {
        /// <summary>
        /// Configures the entity of type <see cref="ProductOrderEntity"/> for the Entity Framework Core model.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity.</param>
        public void Configure(EntityTypeBuilder<ProductOrderEntity> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ToTable("product_order_aggregates");

            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.ProductId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(entity => entity.Quantity)
                .IsRequired();

            // Deliberately not unique: AddOrdersAsync stores one row per submitted order line, and a product
            // keeps its dispatched history alongside any newly pending lines. A unique index here would reject
            // the second order for a product on any provider that enforces it.
            builder.HasIndex(entity => entity.ProductId);

            builder.Property(entity => entity.DispatchedAt);

            // The dispatch cycle filters on pending rows on every tick.
            builder.HasIndex(entity => entity.DispatchedAt);
        }
    }
}
