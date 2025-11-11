namespace Databento.Client.Models.Symbology;

/// <summary>
/// Result of a symbology resolution query
/// Resolves how symbols map between different symbology types across date ranges
/// </summary>
public sealed class SymbologyResolution
{
    /// <summary>
    /// Mappings from input symbols to resolved symbols across date intervals
    /// Key: Input symbol (in stype_in format)
    /// Value: List of intervals showing how the symbol resolves over time
    /// </summary>
    public required IReadOnlyDictionary<string, IReadOnlyList<MappingInterval>> Mappings { get; init; }

    /// <summary>
    /// Symbols that resolved for at least one day but not the entire date range
    /// </summary>
    public required IReadOnlyList<string> Partial { get; init; }

    /// <summary>
    /// Symbols that did not resolve for any day in the date range
    /// </summary>
    public required IReadOnlyList<string> NotFound { get; init; }

    /// <summary>
    /// Input symbology type used in the query
    /// </summary>
    public required SType StypeIn { get; init; }

    /// <summary>
    /// Output symbology type used in the query
    /// </summary>
    public required SType StypeOut { get; init; }

    /// <summary>
    /// Gets a summary of the resolution results
    /// </summary>
    public override string ToString()
    {
        var totalMapped = Mappings.Count;
        var totalPartial = Partial.Count;
        var totalNotFound = NotFound.Count;
        return $"SymbologyResolution: {totalMapped} mapped, {totalPartial} partial, {totalNotFound} not found " +
               $"({StypeIn} → {StypeOut})";
    }
}
