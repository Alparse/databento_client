using System.Text.Json.Serialization;

namespace Databento.Client.Models.Batch;

/// <summary>
/// Description of a batch job
/// </summary>
public sealed class BatchJob
{
    /// <summary>
    /// Unique job identifier
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// User ID who submitted the job
    /// </summary>
    [JsonPropertyName("user_id")]
    public required string UserId { get; init; }

    /// <summary>
    /// Cost in US dollars
    /// </summary>
    [JsonPropertyName("cost_usd")]
    public required double CostUsd { get; init; }

    /// <summary>
    /// Dataset name (e.g., "GLBX.MDP3")
    /// </summary>
    [JsonPropertyName("dataset")]
    public required string Dataset { get; init; }

    /// <summary>
    /// List of symbols requested
    /// </summary>
    [JsonPropertyName("symbols")]
    public required IReadOnlyList<string> Symbols { get; init; }

    /// <summary>
    /// Input symbology type
    /// </summary>
    [JsonPropertyName("stype_in")]
    public required SType StypeIn { get; init; }

    /// <summary>
    /// Output symbology type
    /// </summary>
    [JsonPropertyName("stype_out")]
    public required SType StypeOut { get; init; }

    /// <summary>
    /// Data schema
    /// </summary>
    [JsonPropertyName("schema")]
    public required Schema Schema { get; init; }

    /// <summary>
    /// Start time (ISO 8601 string)
    /// </summary>
    [JsonPropertyName("start")]
    public required string Start { get; init; }

    /// <summary>
    /// End time (ISO 8601 string)
    /// </summary>
    [JsonPropertyName("end")]
    public required string End { get; init; }

    /// <summary>
    /// Maximum number of records (0 = unlimited)
    /// </summary>
    [JsonPropertyName("limit")]
    public required ulong Limit { get; init; }

    /// <summary>
    /// Data encoding format
    /// </summary>
    [JsonPropertyName("encoding")]
    public required Encoding Encoding { get; init; }

    /// <summary>
    /// Compression algorithm
    /// </summary>
    [JsonPropertyName("compression")]
    public required Compression Compression { get; init; }

    /// <summary>
    /// Whether prices are human-readable
    /// </summary>
    [JsonPropertyName("pretty_px")]
    public required bool PrettyPx { get; init; }

    /// <summary>
    /// Whether timestamps are human-readable
    /// </summary>
    [JsonPropertyName("pretty_ts")]
    public required bool PrettyTs { get; init; }

    /// <summary>
    /// Whether to include symbol mappings
    /// </summary>
    [JsonPropertyName("map_symbols")]
    public required bool MapSymbols { get; init; }

    /// <summary>
    /// Time duration for splitting output files
    /// </summary>
    [JsonPropertyName("split_duration")]
    public required SplitDuration SplitDuration { get; init; }

    /// <summary>
    /// Size threshold for splitting files (bytes)
    /// </summary>
    [JsonPropertyName("split_size")]
    public required ulong SplitSize { get; init; }

    /// <summary>
    /// Whether to split files by symbol
    /// </summary>
    [JsonPropertyName("split_symbols")]
    public required bool SplitSymbols { get; init; }

    /// <summary>
    /// Delivery method
    /// </summary>
    [JsonPropertyName("delivery")]
    public required Delivery Delivery { get; init; }

    /// <summary>
    /// Number of records in the job output
    /// </summary>
    [JsonPropertyName("record_count")]
    public required ulong RecordCount { get; init; }

    /// <summary>
    /// Billed size in bytes
    /// </summary>
    [JsonPropertyName("billed_size")]
    public required ulong BilledSize { get; init; }

    /// <summary>
    /// Actual uncompressed size in bytes
    /// </summary>
    [JsonPropertyName("actual_size")]
    public required ulong ActualSize { get; init; }

    /// <summary>
    /// Compressed package size in bytes
    /// </summary>
    [JsonPropertyName("package_size")]
    public required ulong PackageSize { get; init; }

    /// <summary>
    /// Current job state
    /// </summary>
    [JsonPropertyName("state")]
    public required JobState State { get; init; }

    /// <summary>
    /// Time when job was received (ISO 8601 string)
    /// </summary>
    [JsonPropertyName("ts_received")]
    public required string TsReceived { get; init; }

    /// <summary>
    /// Time when job was queued (ISO 8601 string, empty if not queued)
    /// </summary>
    [JsonPropertyName("ts_queued")]
    public required string TsQueued { get; init; }

    /// <summary>
    /// Time when processing started (ISO 8601 string, empty if not started)
    /// </summary>
    [JsonPropertyName("ts_process_start")]
    public required string TsProcessStart { get; init; }

    /// <summary>
    /// Time when processing completed (ISO 8601 string, empty if not done)
    /// </summary>
    [JsonPropertyName("ts_process_done")]
    public required string TsProcessDone { get; init; }

    /// <summary>
    /// Time when job files expire (ISO 8601 string, empty if not done)
    /// </summary>
    [JsonPropertyName("ts_expiration")]
    public required string TsExpiration { get; init; }
}
