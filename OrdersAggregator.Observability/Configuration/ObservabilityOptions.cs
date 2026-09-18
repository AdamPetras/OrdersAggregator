namespace OrdersAggregator.Observability.Configuration;

using System.ComponentModel.DataAnnotations;

using OrdersAggregator.Core.Configuration;

/// <summary>
/// Configures how traces, metrics, and logs are exported over OTLP.
/// </summary>
/// <remarks>The defaults target a local collector on the standard gRPC port, which is what
/// <c>docker-compose.observability.yml</c> exposes. Pointing at Grafana Cloud instead only requires changing
/// <see cref="Endpoint"/>, switching <see cref="Protocol"/> to <see cref="HttpProtobufProtocol"/>, and supplying the
/// tenant credentials through <see cref="Headers"/>.</remarks>
public sealed class ObservabilityOptions : IOptionsWithSectionName, IValidatableObject
{
    /// <summary>
    /// The <see cref="Protocol"/> value selecting OTLP over gRPC.
    /// </summary>
    public const string GrpcProtocol = "grpc";

    /// <summary>
    /// The <see cref="Protocol"/> value selecting OTLP over HTTP with protobuf payloads.
    /// </summary>
    public const string HttpProtobufProtocol = "httpprotobuf";

    /// <inheritdoc/>
    public static string SectionName => "Observability";

    /// <summary>
    /// Gets or sets a value indicating whether OpenTelemetry is wired up at all. When <see langword="false"/> the
    /// application still logs through <c>ILogger</c>, but nothing is exported.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the <c>service.name</c> resource attribute reported to the backend.
    /// </summary>
    public string ServiceName { get; set; } = "orders-aggregator";

    /// <summary>
    /// Gets or sets the <c>service.namespace</c> resource attribute reported to the backend.
    /// </summary>
    public string ServiceNamespace { get; set; } = "ordersaggregator";

    /// <summary>
    /// Gets or sets the <c>service.version</c> resource attribute. When empty the entry assembly version is used.
    /// </summary>
    public string ServiceVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the <c>deployment.environment.name</c> resource attribute. When empty the host environment name is used.
    /// </summary>
    public string DeploymentEnvironment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base OTLP endpoint. Signal-specific paths are appended automatically when
    /// <see cref="Protocol"/> is <see cref="HttpProtobufProtocol"/>.
    /// </summary>
    public string Endpoint { get; set; } = "http://localhost:4317";

    /// <summary>
    /// Gets or sets the OTLP transport, either <see cref="GrpcProtocol"/> or <see cref="HttpProtobufProtocol"/>.
    /// </summary>
    public string Protocol { get; set; } = GrpcProtocol;

    /// <summary>
    /// Gets or sets optional OTLP headers in <c>key=value,key2=value2</c> form, used for backend authentication.
    /// </summary>
    public string Headers { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the head sampling ratio for traces, between <c>0.0</c> and <c>1.0</c>.
    /// </summary>
    public double SamplingRatio { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets a value indicating whether traces are collected and exported.
    /// </summary>
    public bool TracingEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether metrics are collected and exported.
    /// </summary>
    public bool MetricsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether logs are exported over OTLP in addition to the configured providers.
    /// </summary>
    public bool LoggingEnabled { get; set; } = true;

    /// <summary>
    /// Gets a value indicating whether <see cref="Protocol"/> selects the HTTP/protobuf transport.
    /// </summary>
    public bool UsesHttpProtobuf =>
        string.Equals(Protocol, HttpProtobufProtocol, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Validates the exporter configuration.
    /// </summary>
    /// <param name="validationContext">The context in which the validation is performed.</param>
    /// <returns>A collection of validation results describing any errors found.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ServiceName))
        {
            yield return new ValidationResult("A service name is required.", [nameof(ServiceName)]);
        }

        if (SamplingRatio is < 0.0 or > 1.0)
        {
            yield return new ValidationResult(
                "The sampling ratio must be between 0.0 and 1.0.",
                [nameof(SamplingRatio)]);
        }

        if (!Enabled)
        {
            yield break;
        }

        if (!string.Equals(Protocol, GrpcProtocol, StringComparison.OrdinalIgnoreCase) && !UsesHttpProtobuf)
        {
            yield return new ValidationResult(
                $"The OTLP protocol must be '{GrpcProtocol}' or '{HttpProtobufProtocol}'.",
                [nameof(Protocol)]);
        }

        if (!Uri.TryCreate(Endpoint, UriKind.Absolute, out _))
        {
            yield return new ValidationResult(
                "The OTLP endpoint must be an absolute URI.",
                [nameof(Endpoint)]);
        }
    }
}
