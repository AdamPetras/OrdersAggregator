using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersDispatcher.Configuration;
using OrdersDispatcher.Services;

namespace OrdersDispatcher;

/// <summary>
/// Provides extension methods for setting up the OrdersDispatcher services in the dependency injection container. This includes
/// configuring the OrderDispatchOptions and registering the OrderDispatchBackgroundService.
/// </summary>
public static class OrdersDispatcherInstaller
{
    /// <summary>
    /// Adds the OrdersDispatcher services to the specified IServiceCollection. This includes configuring the OrderDispatchOptions.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">The application configuration source.</param>
    public static void AddOrdersDispatcher(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        OrderDispatchOptions orderDispatchOptions = new();
        configuration.GetSection(OrderDispatchOptions.SectionName).Bind(orderDispatchOptions);

        services.AddSingleton(orderDispatchOptions);
        services.AddHostedService<OrderDispatchBackgroundService>();
    }
}