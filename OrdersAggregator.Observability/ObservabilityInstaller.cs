namespace OrdersAggregator.Observability;

using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using OrdersAggregator.Core.Diagnostics;
using OrdersAggregator.Observability.Configuration;

/// <summary>
/// Registers OpenTelemetry tracing, metrics, and log export for the application.
/// </summary>
public static class ObservabilityInstaller
{
    private const string TracesSignalPath = "v1/traces";
    private const string MetricsSignalPath = "v1/metrics";
    private const string LogsSignalPath = "v1/logs";
    private const string DeploymentEnvironmentAttribute = "deployment.environment.name";

    /// <summary>
    /// Adds the observability layer to the service collection.
    /// </summary>
    /// <remarks><see cref="OrdersMetrics"/> is registered whether or not export is enabled, so that instrumented
    /// code can depend on it unconditionally. When <see cref="ObservabilityOptions.Enabled"/> is
    /// <see langword="false"/> the measurements are simply never collected.</remarks>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.ConfigureOptions<ObservabilityOptionsConfiguration>();
        services.AddOrdersDiagnostics();

        ObservabilityOptions options = BindOptions(configuration);
        if (!options.Enabled)
        {
            return;
        }

        IOpenTelemetryBuilder builder = services.AddOpenTelemetry()
            .ConfigureResource(resource => ConfigureResource(resource, options, configuration));

        if (options.TracingEnabled)
        {
            builder.WithTracing(tracing => tracing
                .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(options.SamplingRatio)))
                .AddSource(OrdersTelemetry.ActivitySourceName)
                .AddSource(OrdersTelemetry.EntityFrameworkCoreActivitySourceName)
                .AddAspNetCoreInstrumentation(instrumentation => instrumentation.RecordException = true)
                .AddHttpClientInstrumentation(instrumentation => instrumentation.RecordException = true)
                .AddOtlpExporter(exporter => ConfigureExporter(exporter, options, TracesSignalPath)));
        }

        if (options.MetricsEnabled)
        {
            builder.WithMetrics(metrics => metrics
                .AddMeter(OrdersTelemetry.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(exporter => ConfigureExporter(exporter, options, MetricsSignalPath)));
        }

        if (options.LoggingEnabled)
        {
            builder.WithLogging(
                logging => logging.AddOtlpExporter(exporter => ConfigureExporter(exporter, options, LogsSignalPath)),
                loggerOptions =>
                {
                    // Without these the exported log record carries only the message template and its arguments,
                    // which makes the Loki view much harder to read than the console view.
                    loggerOptions.IncludeFormattedMessage = true;
                    loggerOptions.IncludeScopes = true;
                });
        }
    }

    /// <summary>
    /// Binds the observability options eagerly, because the OpenTelemetry pipeline has to be shaped at
    /// registration time rather than when the options are first resolved.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The bound and validated options.</returns>
    private static ObservabilityOptions BindOptions(IConfiguration configuration)
    {
        ObservabilityOptions options = new();
        new ObservabilityOptionsConfiguration(configuration).Configure(options);
        return options;
    }

    private static void ConfigureResource(
        ResourceBuilder resource,
        ObservabilityOptions options,
        IConfiguration configuration)
    {
        string serviceVersion = string.IsNullOrWhiteSpace(options.ServiceVersion)
            ? Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0"
            : options.ServiceVersion;

        string environmentName = string.IsNullOrWhiteSpace(options.DeploymentEnvironment)
            ? configuration[HostDefaults.EnvironmentKey] ?? Environments.Production
            : options.DeploymentEnvironment;

        resource
            .AddService(
                serviceName: options.ServiceName,
                serviceNamespace: options.ServiceNamespace,
                serviceVersion: serviceVersion,
                autoGenerateServiceInstanceId: true)
            .AddAttributes([new KeyValuePair<string, object>(DeploymentEnvironmentAttribute, environmentName)]);
    }

    private static void ConfigureExporter(
        OtlpExporterOptions exporter,
        ObservabilityOptions options,
        string signalPath)
    {
        exporter.Protocol = options.UsesHttpProtobuf ? OtlpExportProtocol.HttpProtobuf : OtlpExportProtocol.Grpc;
        exporter.Endpoint = BuildEndpoint(options, signalPath);

        if (!string.IsNullOrWhiteSpace(options.Headers))
        {
            exporter.Headers = options.Headers;
        }
    }

    /// <summary>
    /// Builds the per-signal endpoint. OpenTelemetry only appends the signal path when the endpoint comes from
    /// environment variables, so a programmatically assigned HTTP endpoint has to carry the path itself.
    /// </summary>
    /// <param name="options">The bound observability options.</param>
    /// <param name="signalPath">The relative OTLP path for the signal being exported.</param>
    /// <returns>The endpoint the exporter should post to.</returns>
    private static Uri BuildEndpoint(ObservabilityOptions options, string signalPath)
    {
        Uri baseEndpoint = new(options.Endpoint, UriKind.Absolute);
        if (!options.UsesHttpProtobuf)
        {
            return baseEndpoint;
        }

        string basePath = baseEndpoint.AbsoluteUri.EndsWith('/')
            ? baseEndpoint.AbsoluteUri
            : baseEndpoint.AbsoluteUri + "/";

        return new Uri(basePath + signalPath, UriKind.Absolute);
    }
}
