namespace OrdersAggregator.Client.Infrastructure;

using System.Net;

using Microsoft.AspNetCore.Components;

using OrdersAggregator.Client.Pages;

/// <summary>
/// A delegating HTTP handler that intercepts API responses and navigates to a forbidden page when a 403 (Forbidden) status code is encountered.
/// </summary>
public sealed class ApiErrorHandler : DelegatingHandler
{
    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiErrorHandler"/> class with the specified navigation manager.
    /// </summary>
    /// <param name="navigationManager">The NavigationManager instance used to manage URI navigation within the application. Cannot be null.</param>
    public ApiErrorHandler(NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);
        _navigationManager = navigationManager;
    }

    /// <summary>
    /// Sends an HTTP request asynchronously and processes the response, navigating to a forbidden page if the response
    /// status is 403 (Forbidden).
    /// </summary>
    /// <remarks>If the response status code is 403 (Forbidden), this method triggers navigation to a
    /// forbidden page before returning the response.</remarks>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message received
    /// from the server.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            NavigateToForbiddenPage();
        }

        return response;
    }

    private void NavigateToForbiddenPage()
    {
        string forbiddenRoute = Forbidden.PageLink.TrimStart('/');
        string currentPath = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
        if (currentPath.StartsWith(forbiddenRoute, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _navigationManager.NavigateTo(Forbidden.PageLink);
    }
}