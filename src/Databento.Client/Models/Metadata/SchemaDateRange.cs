namespace Databento.Client.Models.Metadata;

/// <summary>
/// Date range for a specific schema
/// </summary>
public class SchemaDateRange
{
    /// <summary>
    /// Start timestamp (ISO 8601)
    /// </summary>
    public string Start { get; set; } = string.Empty;

    /// <summary>
    /// End timestamp (ISO 8601, exclusive)
    /// </summary>
    public string End { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Start} to {End}";
    }
}
