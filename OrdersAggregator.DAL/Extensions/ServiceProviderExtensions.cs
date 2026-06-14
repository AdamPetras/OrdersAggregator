using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OrdersAggregator.DAL.Extensions
{
    /// <summary>
    /// Provides startup helpers for DAL infrastructure.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Ensures that the configured orders database exists before the application starts serving requests.
        /// </summary>
        /// <param name="serviceProvider">The root service provider.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous initialization work.</returns>
        public static async Task EnsureOrdersDatabaseCreatedAsync(
            this IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IDbContextFactory<OrdersDbContext>? dbContextFactory = scope.ServiceProvider.GetService<IDbContextFactory<OrdersDbContext>>();

                if (dbContextFactory is null)
                {
                    return;
                }

                await using (OrdersDbContext dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken))
                {
                    await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                }
            }
        }
    }
}
