namespace OrdersAggregator.Core.Diagnostics;

using System.Diagnostics;

/// <summary>
/// Declares the shared activity source and meter names used across the application so that the code that
/// produces telemetry and the code that registers OpenTelemetry cannot drift apart.
/// </summary>
/// <remarks>This type lives in the Core project because every server-side layer already references it. Adding a
/// new span or metric means using the members here rather than creating a private <see cref="System.Diagnostics.ActivitySource"/>,
/// otherwise the signal is produced but never exported.</remarks>
public static class OrdersTelemetry
{
    /// <summary>
    /// The name of the activity source that emits application spans.
    /// </summary>
    public const string ActivitySourceName = "OrdersAggregator";

    /// <summary>
    /// The name of the meter that emits application metrics.
    /// </summary>
    public const string MeterName = "OrdersAggregator";

    /// <summary>
    /// The name of the activity source Entity Framework Core uses to emit command spans.
    /// </summary>
    public const string EntityFrameworkCoreActivitySourceName = "Microsoft.EntityFrameworkCore";

    /// <summary>
    /// Gets the shared activity source used to start application spans.
    /// </summary>
    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);
}
