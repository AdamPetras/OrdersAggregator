using System.Text.Json;
using System.Text.Json.Serialization;
using OrdersAggregator.Client.Services;
using OrdersAggregator.Contracts.Serialization;
using Refit;

namespace OrdersAggregator.Client.Infrastructure;

/// <summary>
/// Provides a catalog of API client types and extension methods for registering them with dependency injection.
/// </summary>
/// <remarks>The static ApiCatalog class exposes a list of supported API interface types and a method to register
/// all corresponding Refit API clients with the application's service collection. The AddApiClients method configures
/// each client with authentication and error handling message handlers, as well as resilience strategies. This class is
/// intended to simplify the integration and configuration of multiple API clients in applications that use dependency
/// injection.</remarks>
public static class ApiCatalog
{
    /// <summary>
    /// Gets a read-only list of API interface types that represent the available services in the application.
    /// </summary>
    /// <remarks>This list includes interfaces for operations such as rental shop management, item handling,
    /// customer interactions, and related services. It serves as a central reference for discovering and accessing the
    /// different API types provided by the application.</remarks>
    public static readonly IReadOnlyList<Type> ApiTypes =
        [
            typeof(IOrdersApi),
        ];

    /// <summary>
    /// Registers API client interfaces with the specified base address and standard handlers in the service collection.
    /// </summary>
    /// <remarks>This method configures each API client with authentication and error handling message
    /// handlers, and applies standard resilience policies. All API clients are created using Refit with shared settings
    /// and the provided base address. This approach centralizes API client configuration and ensures consistent
    /// behavior across all registered clients.</remarks>
    /// <param name="services">The service collection to which the API clients and related handlers will be added. Cannot be null.</param>
    /// <param name="baseAddress">The base address to assign to all HTTP clients created for the API interfaces. Cannot be null.</param>
    /// <returns>The service collection instance with the API clients and handlers registered. This enables dependency injection
    /// of the API interfaces throughout the application.</returns>
    public static IServiceCollection AddApiClients(this IServiceCollection services, Uri baseAddress)
    {
        services.AddTransient<ApiErrorHandler>();
        RefitSettings refitSettings = CreateRefitSettings();

        foreach (Type apiType in ApiTypes)
        {
            IHttpClientBuilder builder = services.AddRefitClient(apiType, refitSettings)
               .ConfigureHttpClient(client => client.BaseAddress = baseAddress);

            builder.AddHttpMessageHandler<ApiErrorHandler>();
            builder.AddStandardResilienceHandler();
        }

        return services;
    }

    private static RefitSettings CreateRefitSettings()
    {
        RefitSettings settings = new()
            {
                ContentSerializer = new SystemTextJsonContentSerializer(OrderApiJsonSerializer.Create()),
            };

        settings.ExceptionFactory = response => RefitExceptionHandler.HandleAsync(response, settings);
        settings.DeserializationExceptionFactory = (response, exception) =>
            RefitExceptionHandler.HandleDeserializationExceptionAsync(response, exception, settings);

        return settings;
    }
}