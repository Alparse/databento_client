namespace Databento.Client.Models.Symbology;

/// <summary>
/// Represents a time interval during which a symbol mapping is valid
/// </summary>
public sealed class MappingInterval
{
    /// <summary>
    /// Start date of the interval (inclusive)
    /// </summary>
    public required DateOnly StartDate { get; init; }

    /// <summary>
    /// End date of the interval (exclusive)
    /// </summary>
    public required DateOnly EndDate { get; init; }

    /// <summary>
    /// The resolved symbol for this interval (in stype_out format)
    /// </summary>
    public required string Symbol { get; init; }

    /// <summary>
    /// String representation of the mapping interval
    /// </summary>
    public override string ToString() =>
        $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}: {Symbol}";
}
