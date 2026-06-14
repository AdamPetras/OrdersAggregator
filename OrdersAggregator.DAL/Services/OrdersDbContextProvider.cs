namespace OrdersAggregator.DAL.Services;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides access to instances of <see cref="OrdersDbContext"/> for performing database operations related to orders.
/// This class uses a factory pattern to create new instances of the database context as needed, ensuring that each operation gets a fresh context instance.
/// The provider supports both synchronous and asynchronous retrieval of the database context, allowing for flexible usage in various application scenarios.
/// </summary>
public sealed class OrdersDbContextProvider : IOrdersDbContextProvider
{
    private readonly IDbContextFactory<OrdersDbContext> _dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersDbContextProvider"/> class with the specified database context factory.
    /// </summary>
    /// <param name="dbContextFactory">The factory used to create instances of <see cref="OrdersDbContext"/>. Cannot be null.</param>
    public OrdersDbContextProvider(IDbContextFactory<OrdersDbContext> dbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(dbContextFactory);
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc/>
    public OrdersDbContextBase GetDbContext()
    {
        return _dbContextFactory.CreateDbContext();
    }

    /// <inheritdoc/>
    public async Task<OrdersDbContextBase> GetDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContextFactory.CreateDbContextAsync(cancellationToken);
    }
}