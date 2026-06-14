using System.Reflection;
using OrdersAggregator.Contracts.Serialization;
using OrdersAggregator.DAL.Extensions;
using OrdersAggregator.Server.Business;

namespace OrdersAggregator.Server
{
    using OrdersAggregator.DAL;

    using OrdersDispatcher;
    using OrdersDispatcher.Services;

    /// <summary>
    /// Configures and runs the ASP.NET Core web application.
    /// </summary>
    /// <remarks>The Program class serves as the entry point for the application. It sets up services,
    /// configures middleware, and starts the web server. This class is typically not used directly by application code,
    /// but is required for application startup in ASP.NET Core projects.</remarks>
    public class Program
    {
        private const string DevelopmentClientCorsPolicyName = "DevelopmentClient";

        /// <summary>
        /// The main entry point for the application. This method configures the web application builder, sets up services,
        /// configures middleware, and starts the web server.
        /// </summary>
        /// <param name="args">An array of command-line arguments supplied to the application at startup.</param>
        /// <returns>A task that represents the asynchronous operation of running the application.</returns>
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            string[] developmentClientOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
            builder.Services.AddControllers()
                .AddJsonOptions(options => OrderApiJsonSerializer.Apply(options.JsonSerializerOptions));

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDal(builder.Configuration);
            builder.Services.AddBusiness(builder.Configuration);
            builder.Services.AddOrdersDispatcher(builder.Configuration);

            if (developmentClientOrigins.Length > 0)
            {
                builder.Services.AddCors(
                    options => options.AddPolicy(
                        name: DevelopmentClientCorsPolicyName,
                        policyBuilder => policyBuilder
                            .WithOrigins(developmentClientOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()));
            }

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }

            app.UseRouting();

            if (developmentClientOrigins.Length > 0)
            {
                app.UseCors(DevelopmentClientCorsPolicyName);
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("index.html");

            await app.Services.EnsureOrdersDatabaseCreatedAsync();
            await app.RunAsync();
        }
    }
}
