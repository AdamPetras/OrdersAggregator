using Microsoft.Extensions.Configuration;
using OrdersAggregator.Core.Configuration;

namespace OrdersDispatcher.Configuration;

/// <summary>
/// Represents the configuration for OrderDispatchOptions, binding values from the application's configuration source.
/// </summary>
/// <param name="configuration">The application's configuration instance.</param>
public class OrderDispatchOptionsConfiguration(IConfiguration configuration) : OptionsConfigurationBase<OrderDispatchOptions>(configuration);