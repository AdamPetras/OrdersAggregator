namespace OrdersDispatcher.Services
{
    using System.Diagnostics;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using OrdersAggregator.Contracts.Dtos;
    using OrdersAggregator.Core.Diagnostics;
    using OrdersAggregator.Server.Business.Services.Orders;

    using OrdersDispatcher.Configuration;

    /// <summary>
    /// A background service that periodically retrieves pending product order aggregates and dispatches them using the configured dispatcher.
    /// </summary>
    internal sealed class OrderDispatchBackgroundService : BackgroundService
    {
        private const string DispatchCycleActivityName = "orders.dispatch_cycle";

        private readonly IOrderService _orderService;
        private readonly IAggregatedOrderDispatcher _aggregatedOrderDispatcher;
        private readonly OrderDispatchOptions _orderDispatchOptions;
        private readonly OrdersMetrics _metrics;
        private readonly ILogger<OrderDispatchBackgroundService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDispatchBackgroundService"/> class with the specified dependencies.
        /// </summary>
        /// <param name="orderService">The service used to retrieve pending product order aggregates.</param>
        /// <param name="aggregatedOrderDispatcher">The dispatcher used to dispatch aggregated orders.</param>
        /// <param name="orderDispatchOptions">The options that configure the dispatch interval and other settings.</param>
        /// <param name="metrics">The instrumentation used to record dispatch measurements.</param>
        /// <param name="logger">The logger used to record dispatch activity.</param>
        public OrderDispatchBackgroundService(
            IOrderService orderService,
            IAggregatedOrderDispatcher aggregatedOrderDispatcher,
            IOptions<OrderDispatchOptions> orderDispatchOptions,
            OrdersMetrics metrics,
            ILogger<OrderDispatchBackgroundService> logger)
        {
            ArgumentNullException.ThrowIfNull(orderService);
            ArgumentNullException.ThrowIfNull(aggregatedOrderDispatcher);
            ArgumentNullException.ThrowIfNull(orderDispatchOptions);
            ArgumentNullException.ThrowIfNull(metrics);
            ArgumentNullException.ThrowIfNull(logger);

            _orderService = orderService;
            _aggregatedOrderDispatcher = aggregatedOrderDispatcher;
            _orderDispatchOptions = orderDispatchOptions.Value;
            _metrics = metrics;
            _logger = logger;
        }

        /// <summary>
        /// Executes the background service, periodically retrieving pending product order aggregates and dispatching them.
        /// </summary>
        /// <param name="stoppingToken">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation of the background service.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DispatcherLogs.DispatchServiceStarted(_logger, _orderDispatchOptions.DispatchInterval.TotalSeconds);

            try
            {
                using (PeriodicTimer timer = new(_orderDispatchOptions.DispatchInterval))
                {
                    while (await timer.WaitForNextTickAsync(stoppingToken))
                    {
                        await RunDispatchCycleAsync(stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the host shuts down.
            }

            DispatcherLogs.DispatchServiceStopping(_logger);
        }

        /// <summary>
        /// Runs a single dispatch cycle under its own trace, recording the outcome either way.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the cycle.</param>
        /// <returns>A task that represents the asynchronous dispatch cycle.</returns>
        private async Task RunDispatchCycleAsync(CancellationToken cancellationToken)
        {
            using Activity? activity = OrdersTelemetry.ActivitySource.StartActivity(DispatchCycleActivityName);

            long startedAt = Stopwatch.GetTimestamp();
            int productCount = 0;
            bool succeeded = false;

            try
            {
                PendingOrderBatch batch = await _orderService.GetGroupedPendingAsync(cancellationToken);

                productCount = batch.Aggregates.Count;
                activity?.SetTag("orders.product_count", productCount);
                activity?.SetTag("orders.count", batch.OrderIds.Count);

                if (batch.IsEmpty)
                {
                    DispatcherLogs.DispatchCycleEmpty(_logger);
                    succeeded = true;
                    return;
                }

                await _aggregatedOrderDispatcher.DispatchAsync(batch.Aggregates, cancellationToken);

                // Only after the dispatch has actually succeeded, so a failure leaves the rows pending for the
                // next cycle rather than silently dropping them.
                await _orderService.MarkDispatchedAsync(batch.OrderIds, cancellationToken);

                succeeded = true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                // A failed cycle must not tear the hosted service down, otherwise dispatch stops silently for the
                // remaining lifetime of the process.
                DispatcherLogs.DispatchCycleFailed(_logger, exception);
                activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            }
            finally
            {
                _metrics.RecordDispatch(productCount, Stopwatch.GetElapsedTime(startedAt), succeeded);
            }
        }
    }
}
