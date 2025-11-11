using System.Text.Json.Serialization;

namespace Databento.Client.Models.Dbn;

/// <summary>
/// Metadata about a DBN file or stream
/// </summary>
public sealed class DbnMetadata
{
    /// <summary>
    /// The DBN schema version number
    /// </summary>
    [JsonPropertyName("version")]
    public required byte Version { get; init; }

    /// <summary>
    /// The dataset code (e.g., "GLBX.MDP3")
    /// </summary>
    [JsonPropertyName("dataset")]
    public required string Dataset { get; init; }

    /// <summary>
    /// The data record schema. Null for live data with mixed schemas.
    /// </summary>
    [JsonPropertyName("schema")]
    public Schema? Schema { get; init; }

    /// <summary>
    /// The UNIX timestamp of the query start, or the first record if the file was split (nanoseconds)
    /// </summary>
    [JsonPropertyName("start")]
    public required long Start { get; init; }

    /// <summary>
    /// The UNIX timestamp of the query end, or the last record if the file was split (nanoseconds)
    /// </summary>
    [JsonPropertyName("end")]
    public required long End { get; init; }

    /// <summary>
    /// The maximum number of records for the query (0 = unlimited)
    /// </summary>
    [JsonPropertyName("limit")]
    public required ulong Limit { get; init; }

    /// <summary>
    /// The input symbology type. Null for live data with mixed stype_in values.
    /// </summary>
    [JsonPropertyName("stype_in")]
    public SType? StypeIn { get; init; }

    /// <summary>
    /// The output symbology type
    /// </summary>
    [JsonPropertyName("stype_out")]
    public required SType StypeOut { get; init; }

    /// <summary>
    /// Whether the records contain an appended send timestamp
    /// </summary>
    [JsonPropertyName("ts_out")]
    public required bool TsOut { get; init; }

    /// <summary>
    /// The length in bytes of fixed-length symbol strings, including null terminator
    /// </summary>
    [JsonPropertyName("symbol_cstr_len")]
    public required ulong SymbolCstrLen { get; init; }

    /// <summary>
    /// The original query input symbols from the request
    /// </summary>
    [JsonPropertyName("symbols")]
    public required IReadOnlyList<string> Symbols { get; init; }

    /// <summary>
    /// Symbols that did not resolve for at least one day in the query time range
    /// </summary>
    [JsonPropertyName("partial")]
    public required IReadOnlyList<string> Partial { get; init; }

    /// <summary>
    /// Symbols that did not resolve for any day in the query time range
    /// </summary>
    [JsonPropertyName("not_found")]
    public required IReadOnlyList<string> NotFound { get; init; }

    /// <summary>
    /// Symbol mappings containing native symbols and their mapping intervals
    /// </summary>
    [JsonPropertyName("mappings")]
    public required IReadOnlyList<SymbolMapping> Mappings { get; init; }
}
