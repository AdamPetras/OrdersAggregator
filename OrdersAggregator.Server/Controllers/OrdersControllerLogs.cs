namespace OrdersAggregator.Server.Controllers
{
    /// <summary>
    /// Source-generated log messages emitted by <see cref="OrdersController"/>.
    /// </summary>
    /// <remarks>Message templates live here rather than inline so that every field name reaching the log backend is
    /// declared in one place. Named placeholders become queryable attributes in Loki, so they must stay stable.</remarks>
    internal static partial class OrdersControllerLogs
    {
        [LoggerMessage(
            EventId = 3000,
            Level = LogLevel.Warning,
            Message = "Rejected an order submission with a missing request body.")]
        public static partial void MissingRequestBody(ILogger logger);

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Warning,
            Message = "Rejected an order submission that failed validation on {InvalidFieldCount} field(s).")]
        public static partial void SubmissionRejected(ILogger logger, int invalidFieldCount);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Information,
            Message = "Accepted an order submission of {AcceptedOrderCount} order line(s).")]
        public static partial void SubmissionAccepted(ILogger logger, int acceptedOrderCount);
    }
}
