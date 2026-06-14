using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersAggregator.DAL.Configuration;

namespace OrdersAggregator.DAL;

/// <summary>
/// Provides database access for order aggregation persistence.
/// </summary>
public class OrdersDbContext : OrdersDbContextBase
{
    private readonly ILogger<OrdersDbContext> _logger;

    private readonly ConnectionStringOptions _connectionStringOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersDbContext"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="connectionStringOptions">The connection string options.</param>
    public OrdersDbContext(ILogger<OrdersDbContext> logger, IOptions<ConnectionStringOptions> connectionStringOptions)
    {
        _logger = logger;
        _connectionStringOptions = connectionStringOptions.Value;
    }

    /// <summary>
    /// Configures the database context options, including the connection string and logging behavior. If the options are not already configured, it retrieves the connection string from the provided configuration options and sets up the context to use PostgresSQL with sensitive data logging enabled. If the connection string is missing or invalid, an exception is thrown to indicate the misconfiguration.
    /// </summary>
    /// <param name="optionsBuilder">The options builder used to configure the context.</param>
    /// <exception cref="InvalidOperationException">Thrown when the connection string is not configured.</exception>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            string connStr = _connectionStringOptions.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("Connection string 'DockerDB' is not configured.");
            }

            optionsBuilder
               .EnableSensitiveDataLogging()
               .UseNpgsql(connStr)
               .LogTo(s => _logger.LogDebug(s));
        }
    }
}