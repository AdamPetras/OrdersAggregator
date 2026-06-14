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

            builder.HasIndex(entity => entity.ProductId)
                .IsUnique();
        }
    }
}
