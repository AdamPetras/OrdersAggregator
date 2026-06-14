namespace OrdersAggregator.DAL.Entities
{
    /// <summary>
    /// Represents the current aggregated pending quantity for a single product.
    /// </summary>
    public sealed class ProductOrderEntity
    {
        /// <summary>
        /// Gets or sets the stable identifier of the aggregate row.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the external product identifier that orders are aggregated by.
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the aggregate row was created.
        /// </summary>
        public long Quantity { get; set; }
    }
}
