namespace OrdersDispatcher.Configuration;

using System.ComponentModel.DataAnnotations;

using OrdersAggregator.Core.Configuration;

/// <summary>
/// Configures how often pending order aggregates are dispatched.
/// </summary>
public sealed class OrderDispatchOptions : IOptionsWithSectionName, IValidatableObject
{
    /// <summary>
    /// The minimum supported dispatch interval in seconds.
    /// </summary>
    public const int MinimumDispatchIntervalSeconds = 20;

    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public static string SectionName => "OrderDispatch";

    /// <summary>
    /// Gets or sets the dispatch interval in seconds.
    /// </summary>
    public int DispatchIntervalSeconds { get; set; } = MinimumDispatchIntervalSeconds;

    /// <summary>
    /// Gets the configured dispatch interval.
    /// </summary>
    public TimeSpan DispatchInterval => TimeSpan.FromSeconds(DispatchIntervalSeconds);

    /// <summary>
    /// Validates the dispatch interval to ensure it meets the minimum required value. If the configured dispatch interval is less than the defined minimum, a validation error is returned indicating that the dispatch interval must be at least the specified number of seconds.
    /// </summary>
    /// <param name="validationContext">The context in which the validation is performed.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DispatchIntervalSeconds < MinimumDispatchIntervalSeconds)
        {
            yield return new ValidationResult(
                $"The dispatch interval must be at least {MinimumDispatchIntervalSeconds} seconds.");
        }
    }
}