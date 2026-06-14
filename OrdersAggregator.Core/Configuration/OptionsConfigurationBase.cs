using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace OrdersAggregator.Core.Configuration;

/// <summary>
/// Provides a base class for configuring strongly typed options of type T using an IConfiguration source. Supports
/// setting default values and validating options after binding.
/// </summary>
/// <remarks>Derived classes should override the ConfigureDefaults method to specify default values for the
/// options before binding from configuration. The Configure method binds the options to the configuration section
/// specified by T.SectionName and validates the resulting object using data annotations.</remarks>
/// <typeparam name="T">The type of options to configure. Must implement the IOptionsWithSectionName interface to specify the configuration
/// section name.</typeparam>
/// <param name="configuration">The configuration source used to bind values to the options instance.</param>
public abstract class OptionsConfigurationBase<T>(IConfiguration configuration) : IConfigureOptions<T>
    where T : class, IOptionsWithSectionName
{
    /// <summary>
    /// Configures the specified options instance by applying default values, binding configuration settings, and
    /// validating the resulting object.
    /// </summary>
    /// <remarks>This method retrieves configuration values from the section specified by the options type and
    /// ensures that all properties of the options object are set according to validation rules. An exception is thrown
    /// if validation fails.</remarks>
    /// <param name="options">The options instance to configure. This parameter must not be null and should be an object with properties that
    /// can be bound and validated.</param>
    public void Configure(T options)
    {
        ConfigureDefaults(options);
        configuration.GetSection(T.SectionName).Bind(options);
        Validator.ValidateObject(options, new ValidationContext(options), validateAllProperties: true);
    }

    /// <summary>
    /// Configures the default settings for the specified options instance.
    /// </summary>
    /// <remarks>Override this method in a derived class to provide custom default configuration for the
    /// options. Ensure that the options instance is properly initialized before calling this method.</remarks>
    /// <param name="options">The options instance to configure. Cannot be null.</param>
    protected virtual void ConfigureDefaults(T options)
    {
    }
}