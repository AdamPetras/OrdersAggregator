namespace OrdersAggregator.Server.Business.Services.Orders;

using OrdersAggregator.Contracts.Dtos;

/// <summary>
/// A snapshot of the order lines awaiting dispatch, together with the identifiers needed to mark exactly those
/// lines as dispatched once the payload has been handed over.
/// </summary>
/// <remarks>Reading and marking are deliberately separate steps. Marking before a successful dispatch would
/// drop orders whenever the dispatcher failed, so the pipeline keeps at-least-once semantics: a failed dispatch
/// leaves the rows pending and the next cycle retries them.</remarks>
/// <param name="Aggregates">The pending quantities summed per product.</param>
/// <param name="OrderIds">The identifiers of the order lines the aggregates were computed from.</param>
public sealed record PendingOrderBatch(
    IReadOnlyCollection<ProductOrderAggregatedDto> Aggregates,
    IReadOnlyCollection<Guid> OrderIds)
{
    /// <summary>
    /// Gets an empty batch.
    /// </summary>
    public static PendingOrderBatch Empty { get; } = new([], []);

    /// <summary>
    /// Gets a value indicating whether there is nothing to dispatch.
    /// </summary>
    public bool IsEmpty => Aggregates.Count == 0;
}
