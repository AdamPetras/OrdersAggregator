namespace OrdersAggregator.Client.Infrastructure;

using Refit;

internal static class RefitExceptionHandler
{
    /// <summary>
    /// Creates an exception for unsuccessful HTTP responses.
    /// </summary>
    /// <param name="response">HTTP response message.</param>
    /// <param name="settings">Refit settings.</param>
    /// <returns>An exception for failed responses; otherwise <see langword="null"/> when successful.</returns>
    public static async Task<Exception?> HandleAsync(HttpResponseMessage response, RefitSettings settings)
    {
        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        HttpRequestMessage request = response.RequestMessage ?? new HttpRequestMessage();
        HttpMethod method = request.Method ?? HttpMethod.Get;

        return await ApiException.Create(request, method, response, settings, null);
    }

    /// <summary>
    /// Creates an exception for deserialization failures.
    /// </summary>
    /// <param name="response">HTTP response message.</param>
    /// <param name="exception">Deserialization exception.</param>
    /// <param name="settings">Refit settings.</param>
    /// <returns>An exception representing the failed response and deserialization error.</returns>
    public static async Task<Exception?> HandleDeserializationExceptionAsync(
        HttpResponseMessage response,
        Exception exception,
        RefitSettings settings)
    {
        HttpRequestMessage request = response.RequestMessage ?? new HttpRequestMessage();
        HttpMethod method = request.Method ?? HttpMethod.Get;

        return await ApiException.Create(request, method, response, settings, exception);
    }
}