namespace OrdersAggregator.Server.Business.Services.Orders
{
    /// <summary>
    /// Represents validation failures found in a submitted order batch.
    /// </summary>
    public sealed class OrderSubmissionValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSubmissionValidationException"/> class.
        /// </summary>
        /// <param name="errors">The validation errors keyed by payload path.</param>
        public OrderSubmissionValidationException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more submitted orders are invalid.")
        {
            ArgumentNullException.ThrowIfNull(errors);
            Errors = errors;
        }

        /// <summary>
        /// Gets the validation errors keyed by payload path.
        /// </summary>
        public IReadOnlyDictionary<string, string[]> Errors { get; }
    }
}
