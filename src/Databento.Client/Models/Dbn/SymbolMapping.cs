using System.Text.Json.Serialization;

namespace Databento.Client.Models.Dbn;

/// <summary>
/// Symbol mapping containing a raw symbol and its mapping intervals
/// </summary>
public sealed class SymbolMapping
{
    /// <summary>
    /// The stype_in symbol
    /// </summary>
    [JsonPropertyName("raw_symbol")]
    public required string RawSymbol { get; init; }

    /// <summary>
    /// The mappings of raw_symbol to stype_out for different date ranges
    /// </summary>
    [JsonPropertyName("intervals")]
    public required IReadOnlyList<MappingInterval> Intervals { get; init; }
}
