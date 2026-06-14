namespace OrdersAggregator.DAL.Services;

/// <summary>
/// Defines a contract for providing instances of <see cref="OrdersDbContextBase"/>. This interface abstracts the creation of the database context, allowing for flexibility in how contexts are instantiated and configured. Implementations of this interface can manage the lifecycle of the context, ensure proper configuration, and provide additional functionality such as logging or context pooling if needed.
/// </summary>
public interface IOrdersDbContextProvider
{
    /// <summary>
    /// Creates and returns a new instance of <see cref="OrdersDbContextBase"/>. Implementations of this method should ensure that the returned context is properly configured.
    /// </summary>
    /// <returns>A new instance of <see cref="OrdersDbContextBase"/>.</returns>
    OrdersDbContextBase GetDbContext();

    /// <summary>
    /// Asynchronously creates and returns a new instance of <see cref="OrdersDbContextBase"/>. Implementations of this method should ensure that the returned context is properly configured.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains the new instance of <see cref="OrdersDbContextBase"/>.</returns>
    Task<OrdersDbContextBase> GetDbContextAsync(CancellationToken cancellationToken = default);
}