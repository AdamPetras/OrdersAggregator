namespace OrdersAggregator.Server.Business.Services.Orders;

/// <summary>
/// Summarizes an accepted order submission.
/// </summary>
/// <param name="AcceptedOrderCount">The number of accepted order lines.</param>
public sealed record OrderSubmissionResult(int AcceptedOrderCount);
