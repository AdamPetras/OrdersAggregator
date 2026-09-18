using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrdersAggregator.Core.Diagnostics;
using OrdersAggregator.Server.Business.Services.Orders;

namespace OrdersAggregator.Server.Business
{
    /// <summary>
    /// Registers business-layer services for order intake and dispatch.
    /// </summary>
    public static class BusinessInstaller
    {
        /// <summary>
        /// Registers the business services required to accept and periodically dispatch product orders.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <param name="configuration">The application configuration source.</param>
        public static void AddBusiness(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddOrdersDiagnostics();
            services.TryAddSingleton(TimeProvider.System);

            services.AddSingleton<IOrderService, OrderService>();
        }
    }
}
