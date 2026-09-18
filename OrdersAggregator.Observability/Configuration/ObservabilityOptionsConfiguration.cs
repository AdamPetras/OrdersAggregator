namespace OrdersAggregator.Observability.Configuration;

using Microsoft.Extensions.Configuration;

using OrdersAggregator.Core.Configuration;

/// <summary>
/// Binds and validates <see cref="ObservabilityOptions"/> from the <c>Observability</c> configuration section.
/// </summary>
/// <param name="configuration">The application configuration.</param>
public sealed class ObservabilityOptionsConfiguration(IConfiguration configuration)
    : OptionsConfigurationBase<ObservabilityOptions>(configuration);
