namespace Databento.Client.Models.Metadata;

/// <summary>
/// Pricing feed mode (different from subscription FeedMode)
/// </summary>
public enum PricingMode : byte
{
    /// <summary>
    /// Historical batch data
    /// </summary>
    Historical = 0,

    /// <summary>
    /// Historical streaming data
    /// </summary>
    HistoricalStreaming = 1,

    /// <summary>
    /// Live streaming data
    /// </summary>
    Live = 2
}

/// <summary>
/// Unit prices per schema for a specific feed mode
/// </summary>
public sealed class UnitPricesForMode
{
    /// <summary>
    /// The pricing mode (Historical, HistoricalStreaming, or Live)
    /// </summary>
    public required PricingMode Mode { get; init; }

    /// <summary>
    /// Unit prices per schema (price per gigabyte or per record depending on mode)
    /// </summary>
    public required IReadOnlyDictionary<Schema, decimal> UnitPrices { get; init; }

    /// <summary>
    /// Gets a summary of the pricing
    /// </summary>
    public override string ToString()
    {
        return $"{Mode}: {UnitPrices.Count} schemas";
    }
}
