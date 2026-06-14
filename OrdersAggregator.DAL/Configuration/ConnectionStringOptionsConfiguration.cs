using Microsoft.Extensions.Configuration;
using OrdersAggregator.Core.Configuration;

namespace OrdersAggregator.DAL.Configuration;

/// <summary>
/// Provides configuration for connection string options by binding them to the appropriate section in the application's configuration.
/// </summary>
/// <param name="configuration">The application's configuration instance.</param>
public class ConnectionStringOptionsConfiguration(IConfiguration configuration) : OptionsConfigurationBase<ConnectionStringOptions>(configuration);