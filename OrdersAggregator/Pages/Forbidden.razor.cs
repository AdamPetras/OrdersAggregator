namespace OrdersAggregator.Client.Pages
{
    /// <summary>
    /// Represents the forbidden route page.
    /// </summary>
    public partial class Forbidden
    {
        /// <summary>
        /// Gets the canonical page link.
        /// </summary>
        public static string PageLink => "/error/forbidden";

        /// <summary>
        /// Navigates the user to the home page.
        /// </summary>
        private void NavigateHome()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
