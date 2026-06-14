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

        /// <summary>
        /// Gets or sets when the aggregate row was last updated, which corresponds to when the most recent order for the product was received. This timestamp is used to determine which product aggregates are pending and should be included in the next dispatch batch.
        /// </summary>
        public DateTimeOffset? DispatchedAt { get; set; }
    }
}
