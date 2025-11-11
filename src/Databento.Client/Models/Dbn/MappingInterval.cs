using System.Text.Json.Serialization;

namespace Databento.Client.Models.Dbn;

/// <summary>
/// A symbol mapping interval for a specific date range
/// </summary>
public sealed class MappingInterval
{
    /// <summary>
    /// The start date of the interval (inclusive) in ISO 8601 format
    /// </summary>
    [JsonPropertyName("start_date")]
    public required string StartDate { get; init; }

    /// <summary>
    /// The end date of the interval (exclusive) in ISO 8601 format
    /// </summary>
    [JsonPropertyName("end_date")]
    public required string EndDate { get; init; }

    /// <summary>
    /// The resolved symbol for this interval (in stype_out)
    /// </summary>
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }
}
