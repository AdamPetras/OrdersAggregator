namespace OrdersAggregator.Contracts.Serialization;

using System.Text.Json;

/// <summary>
/// Provides the shared JSON serializer configuration used by the Orders Aggregator API and client.
/// </summary>
public static class OrderApiJsonSerializer
{
    /// <summary>
    /// Creates a new <see cref="JsonSerializerOptions"/> instance configured with the API defaults.
    /// </summary>
    /// <returns>A new serializer options instance using the shared API settings.</returns>
    public static JsonSerializerOptions Create()
    {
        return new(JsonSerializerDefaults.Web);
    }

    /// <summary>
    /// Applies the shared API JSON configuration to an existing <see cref="JsonSerializerOptions"/> instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public static void Apply(JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        JsonSerializerOptions source = Create();
        options.PropertyNamingPolicy = source.PropertyNamingPolicy;
        options.PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive;
        options.DictionaryKeyPolicy = source.DictionaryKeyPolicy;
        options.NumberHandling = source.NumberHandling;
        options.DefaultIgnoreCondition = source.DefaultIgnoreCondition;
    }
}
