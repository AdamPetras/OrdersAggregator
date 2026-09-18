namespace OrdersAggregator.Core.Diagnostics;

using System.Diagnostics.Metrics;

/// <summary>
/// Publishes the order lifecycle metrics exported under the <see cref="OrdersTelemetry.MeterName"/> meter.
/// </summary>
/// <remarks>Registered as a singleton by the observability installer. Instruments are created from
/// <see cref="IMeterFactory"/> rather than a static <see cref="Meter"/> so that tests can resolve an isolated
/// meter and assert on emitted measurements.</remarks>
public sealed class OrdersMetrics
{
    private readonly Counter<long> _submittedOrders;
    private readonly Counter<long> _submittedQuantity;
    private readonly Counter<long> _dispatchedProducts;
    private readonly Histogram<double> _dispatchDuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The factory used to create the application meter.</param>
    public OrdersMetrics(IMeterFactory meterFactory)
    {
        ArgumentNullException.ThrowIfNull(meterFactory);

        Meter meter = meterFactory.Create(OrdersTelemetry.MeterName);

        _submittedOrders = meter.CreateCounter<long>(
            "orders.submitted",
            unit: "{order}",
            description: "Number of order lines accepted by the API.");

        _submittedQuantity = meter.CreateCounter<long>(
            "orders.submitted.quantity",
            unit: "{item}",
            description: "Total product quantity accepted by the API.");

        _dispatchedProducts = meter.CreateCounter<long>(
            "orders.dispatched.products",
            unit: "{product}",
            description: "Number of aggregated product rows handed to the dispatcher.");

        _dispatchDuration = meter.CreateHistogram<double>(
            "orders.dispatch.duration",
            unit: "ms",
            description: "Duration of a single dispatch cycle.");
    }

    /// <summary>
    /// Records an accepted order submission.
    /// </summary>
    /// <param name="orderCount">The number of order lines accepted.</param>
    /// <param name="totalQuantity">The total quantity across the accepted order lines.</param>
    public void RecordSubmission(int orderCount, long totalQuantity)
    {
        _submittedOrders.Add(orderCount);
        _submittedQuantity.Add(totalQuantity);
    }

    /// <summary>
    /// Records the outcome of a completed dispatch cycle.
    /// </summary>
    /// <param name="productCount">The number of aggregated product rows dispatched.</param>
    /// <param name="duration">The wall-clock duration of the dispatch cycle.</param>
    /// <param name="succeeded">Whether the dispatch cycle completed without error.</param>
    public void RecordDispatch(int productCount, TimeSpan duration, bool succeeded)
    {
        KeyValuePair<string, object?> outcome = new("dispatch.outcome", succeeded ? "success" : "failure");

        _dispatchedProducts.Add(productCount, outcome);
        _dispatchDuration.Record(duration.TotalMilliseconds, outcome);
    }
}
