using System.ComponentModel.DataAnnotations;
using Npgsql;
using OrdersAggregator.Core.Configuration;

namespace OrdersAggregator.DAL.Configuration;

/// <summary>
/// Represents configuration options for database connection strings used by the application.
/// </summary>
/// <remarks>This class is typically bound to the 'ConnectionStrings' section of the application's configuration
/// and provides validation for required connection string properties. It is used to ensure that the ConnectionString connection
/// string is present and contains all necessary components for establishing a PostgreSQL database connection.</remarks>
public class ConnectionStringOptions : IOptionsWithSectionName, IValidatableObject
{
    /// <inheritdoc/>
    public static string SectionName => "DbConnection";

    /// <summary>
    /// Gets or sets the connection string used to connect to the PostgreSQL database. This property is required and must include valid Host, Database, Username, and Password components for successful validation.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to use an in-memory database instead of PostgreSQL. If set to true, the ConnectionString property is ignored and no validation is performed on it. This allows for flexibility in testing or development scenarios where a real database connection is not required.
    /// </summary>
    public bool UseInMemory { get; set; }

    /// <summary>
    /// Validates the ConnectionString connection string and returns any validation errors found.
    /// </summary>
    /// <remarks>Validation checks include presence and correctness of required connection string components
    /// such as Host, Database, Username, and Password. If the connection string is malformed or missing required parts,
    /// corresponding errors are returned.</remarks>
    /// <param name="validationContext">The context information about the object being validated. Used to provide additional information during
    /// validation.</param>
    /// <returns>A collection of validation results describing any errors found. The collection is empty if the connection string
    /// is valid.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UseInMemory == true)
        {
            return Array.Empty<ValidationResult>();
        }

        List<ValidationResult> errors = new List<ValidationResult>();
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            errors.Add(new ValidationResult("ConnectionString connection string is required.", new[] { nameof(ConnectionString) }));
        }
        else
        {
            try
            {
                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(ConnectionString);
                if (string.IsNullOrWhiteSpace(builder.Host))
                {
                    errors.Add(new ValidationResult("ConnectionString connection string must include a valid Host.", new[] { nameof(ConnectionString) }));
                }

                if (string.IsNullOrWhiteSpace(builder.Database))
                {
                    errors.Add(new ValidationResult("ConnectionString connection string must include a valid Database.", new[] { nameof(ConnectionString) }));
                }

                if (string.IsNullOrWhiteSpace(builder.Username))
                {
                    errors.Add(new ValidationResult("ConnectionString connection string must include a valid Username.", new[] { nameof(ConnectionString) }));
                }

                if (string.IsNullOrWhiteSpace(builder.Password))
                {
                    errors.Add(new ValidationResult("ConnectionString connection string must include a valid Password.", new[] { nameof(ConnectionString) }));
                }
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationResult($"ConnectionString connection string is invalid: {ex.Message}", new[] { nameof(ConnectionString) }));
            }
        }

        return errors;
    }
}