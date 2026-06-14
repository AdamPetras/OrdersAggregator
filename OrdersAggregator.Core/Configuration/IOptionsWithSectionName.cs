namespace OrdersAggregator.Core.Configuration;

/// <summary>
/// Defines a contract for options classes to specify the name of the configuration section to which they are bound.
/// </summary>
/// <remarks>Implementing this interface allows an options class to indicate the configuration section name used
/// during binding. This facilitates structured configuration management and enables consistent retrieval of options
/// from configuration providers.</remarks>
public interface IOptionsWithSectionName
{
    /// <summary>
    /// Gets the name of the configuration section associated with the implementing type.
    /// </summary>
    /// <remarks>Implementing types must provide a unique section name to identify their configuration
    /// section. This property is intended for use in scenarios such as configuration binding, logging, or diagnostics
    /// where the section name is required.</remarks>
    static abstract string SectionName { get; }
}