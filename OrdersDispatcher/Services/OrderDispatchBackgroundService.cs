namespace OrdersDispatcher.Services
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    using OrdersAggregator.Contracts.Dtos;
    using OrdersAggregator.Server.Business.Services.Orders;

    using OrdersDispatcher.Configuration;

    /// <summary>
    /// A background service that periodically retrieves pending product order aggregates and dispatches them using the configured dispatcher.
    /// </summary>
    internal sealed class OrderDispatchBackgroundService : BackgroundService
    {
        private readonly IOrderService _orderService;
        private readonly IAggregatedOrderDispatcher _aggregatedOrderDispatcher;
        private readonly OrderDispatchOptions _orderDispatchOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDispatchBackgroundService"/> class with the specified dependencies.
        /// </summary>
        /// <param name="orderService">The service used to retrieve pending product order aggregates.</param>
        /// <param name="aggregatedOrderDispatcher">The dispatcher used to dispatch aggregated orders.</param>
        /// <param name="orderDispatchOptions">The options that configure the dispatch interval and other settings.</param>
        public OrderDispatchBackgroundService(
            IOrderService orderService,
            IAggregatedOrderDispatcher aggregatedOrderDispatcher,
            IOptions<OrderDispatchOptions> orderDispatchOptions)
        {
            ArgumentNullException.ThrowIfNull(orderService);
            ArgumentNullException.ThrowIfNull(aggregatedOrderDispatcher);
            ArgumentNullException.ThrowIfNull(orderDispatchOptions);

            _orderService = orderService;
            _aggregatedOrderDispatcher = aggregatedOrderDispatcher;
            _orderDispatchOptions = orderDispatchOptions.Value;
        }

        /// <summary>
        /// Executes the background service, periodically retrieving pending product order aggregates and dispatching them.
        /// </summary>
        /// <param name="stoppingToken">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation of the background service.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new(_orderDispatchOptions.DispatchInterval))
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    IReadOnlyCollection<ProductOrderAggregatedDto> pendingAggregates =
                        await _orderService.TakeGroupedPendingAsync(stoppingToken);

                    if (pendingAggregates.Count == 0)
                    {
                        continue;
                    }

                    await _aggregatedOrderDispatcher.DispatchAsync(pendingAggregates, stoppingToken);
                }
            }
        }
    }
}
