namespace OrdersDispatcher.Services
{
    using Microsoft.Extensions.Hosting;

    using OrdersAggregator.Contracts.Dtos;
    using OrdersAggregator.Server.Business.Services.Orders;

    using OrdersDispatcher.Configuration;

    internal sealed class OrderDispatchBackgroundService : BackgroundService
    {
        private readonly IOrderService _orderService;
        private readonly IAggregatedOrderDispatcher _aggregatedOrderDispatcher;
        private readonly OrderDispatchOptions _orderDispatchOptions;

        public OrderDispatchBackgroundService(
            IOrderService orderService,
            IAggregatedOrderDispatcher aggregatedOrderDispatcher,
            OrderDispatchOptions orderDispatchOptions)
        {
            ArgumentNullException.ThrowIfNull(orderService);
            ArgumentNullException.ThrowIfNull(aggregatedOrderDispatcher);
            ArgumentNullException.ThrowIfNull(orderDispatchOptions);

            _orderService = orderService;
            _aggregatedOrderDispatcher = aggregatedOrderDispatcher;
            _orderDispatchOptions = orderDispatchOptions;
        }

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
