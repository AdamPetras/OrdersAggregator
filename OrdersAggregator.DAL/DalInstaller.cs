using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersAggregator.DAL.Configuration;

namespace OrdersAggregator.DAL
{
    using OrdersAggregator.DAL.Services;

    /// <summary>
    /// Registers DAL services, including the orders database context and persistence options.
    /// </summary>
    public static class DalInstaller
    {
        /// <summary>
        /// Registers the orders database context and persistence options with the dependency injection container. The method reads the persistence options from the provided configuration, registers them as a singleton service, and configures the database context based on the specified provider (PostgresSQL or in-memory).
        /// </summary>
        /// <param name="services">The service collection to which the DAL services will be added.</param>
        /// <param name="configuration">The application configuration from which persistence options will be read.</param>
        public static void AddDal(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            ConnectionStringOptions options = new ConnectionStringOptions();
            configuration.GetSection(ConnectionStringOptions.SectionName).Bind(options);

            services.AddScoped<IOrdersDbContextProvider, OrdersDbContextProvider>();
            services.AddSingleton(options);

            if (options.UseInMemory)
            {
                return;
            }

            services.AddPooledDbContextFactory<OrdersDbContext>(dbContextOptions => ConfigureDbContext(dbContextOptions, options));
        }

        /// <summary>
        /// Configures the database context options for PostgresSQL-backed persistence.
        /// </summary>
        /// <param name="dbContextOptions">The database context options builder to configure.</param>
        /// <param name="options">The persistence options to use for configuration.</param>
        private static void ConfigureDbContext(
            DbContextOptionsBuilder dbContextOptions,
            ConnectionStringOptions options)
        {
            ArgumentNullException.ThrowIfNull(dbContextOptions);
            ArgumentNullException.ThrowIfNull(options);

            ArgumentException.ThrowIfNullOrWhiteSpace(options.ConnectionString);
            dbContextOptions.UseNpgsql(options.ConnectionString);
        }
    }
}
