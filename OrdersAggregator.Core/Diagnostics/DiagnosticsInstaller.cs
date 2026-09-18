namespace OrdersAggregator.Core.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Registers the instrumentation primitives that the server-side layers emit telemetry through.
/// </summary>
/// <remarks>Every installer that resolves <see cref="OrdersMetrics"/> calls this, so each layer stays
/// independently resolvable instead of silently depending on the observability layer having been installed first.
/// The registrations are idempotent.</remarks>
public static class DiagnosticsInstaller
{
    /// <summary>
    /// Adds the shared metrics instrumentation to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The same service collection, to allow chaining.</returns>
    public static IServiceCollection AddOrdersDiagnostics(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMetrics();
        services.TryAddSingleton<OrdersMetrics>();

        return services;
    }
}
