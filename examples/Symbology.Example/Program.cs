using Databento.Client.Builders;
using Databento.Client.Models;

Console.WriteLine("=== Databento Symbology Resolution Example ===");
Console.WriteLine();
Console.WriteLine("This example demonstrates how to resolve symbols between different");
Console.WriteLine("symbology types (e.g., raw symbols to instrument IDs).");
Console.WriteLine();

var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
    ?? throw new InvalidOperationException(
        "DATABENTO_API_KEY environment variable is not set. " +
        "Set it with your API key to authenticate.");

var client = new HistoricalClientBuilder()
    .WithApiKey(apiKey)
    .Build();

Console.WriteLine("✅ Successfully created Historical client");
Console.WriteLine();

// ============================================================================
// Example 1: Resolve Raw Symbols to Instrument IDs
// ============================================================================

Console.WriteLine("Example 1: Resolve Raw Symbols to Instrument IDs");
Console.WriteLine("-------------------------------------------------");
Console.WriteLine("Convert human-readable symbols like 'NVDA' to instrument IDs.");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    string[] symbols = ["NVDA"];
    var startDate = new DateOnly(2024, 1, 1);
    var endDate = new DateOnly(2024, 1, 31);

    Console.WriteLine($"Dataset: {dataset}");
    Console.WriteLine($"Symbols: {string.Join(", ", symbols)}");
    Console.WriteLine($"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
    Console.WriteLine($"Converting: RawSymbol → InstrumentId");
    Console.WriteLine();

    var resolution = await client.SymbologyResolveAsync(
        dataset,
        symbols,
        SType.RawSymbol,
        SType.InstrumentId,
        startDate,
        endDate);

    Console.WriteLine($"Resolution Summary: {resolution}");
    Console.WriteLine();

    // Show successful mappings
    if (resolution.Mappings.Count > 0)
    {
        Console.WriteLine("Successful Mappings:");
        foreach (var (inputSymbol, intervals) in resolution.Mappings)
        {
            Console.WriteLine($"  {inputSymbol}:");
            foreach (var interval in intervals)
            {
                Console.WriteLine($"    {interval.StartDate:yyyy-MM-dd} to {interval.EndDate:yyyy-MM-dd} → {interval.Symbol}");
            }
        }
        Console.WriteLine();
    }

    // Show partial matches (symbols that only resolved for part of the date range)
    if (resolution.Partial.Count > 0)
    {
        Console.WriteLine("Partial Matches (resolved for some but not all days):");
        foreach (var symbol in resolution.Partial)
        {
            Console.WriteLine($"  - {symbol}");
        }
        Console.WriteLine();
    }

    // Show not found symbols
    if (resolution.NotFound.Count > 0)
    {
        Console.WriteLine("Not Found (did not resolve for any day):");
        foreach (var symbol in resolution.NotFound)
        {
            Console.WriteLine($"  - {symbol}");
        }
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error resolving symbols: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 2: Resolve Instrument IDs Back to Raw Symbols
// ============================================================================

Console.WriteLine("Example 2: Resolve Instrument IDs to Raw Symbols");
Console.WriteLine("------------------------------------------------");
Console.WriteLine("Convert instrument IDs back to human-readable symbols.");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    // Use an instrument ID we likely got from Example 1
    // Note: This is a placeholder - in practice you'd use the actual ID from Example 1
    string[] instrumentIds = ["11667"];  // Example instrument ID
    var startDate = new DateOnly(2024, 1, 15);
    var endDate = new DateOnly(2024, 1, 16);

    Console.WriteLine($"Dataset: {dataset}");
    Console.WriteLine($"Instrument IDs: {string.Join(", ", instrumentIds)}");
    Console.WriteLine($"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
    Console.WriteLine($"Converting: InstrumentId → RawSymbol");
    Console.WriteLine();

    var resolution = await client.SymbologyResolveAsync(
        dataset,
        instrumentIds,
        SType.InstrumentId,
        SType.RawSymbol,
        startDate,
        endDate);

    Console.WriteLine($"Resolution Summary: {resolution}");
    Console.WriteLine();

    if (resolution.Mappings.Count > 0)
    {
        Console.WriteLine("Resolved Symbols:");
        foreach (var (inputId, intervals) in resolution.Mappings)
        {
            Console.WriteLine($"  Instrument ID {inputId}:");
            foreach (var interval in intervals)
            {
                Console.WriteLine($"    {interval.StartDate:yyyy-MM-dd} to {interval.EndDate:yyyy-MM-dd} → {interval.Symbol}");
            }
        }
        Console.WriteLine();
    }
    else if (resolution.NotFound.Count > 0)
    {
        Console.WriteLine("Note: Instrument ID not found or not valid for this date range.");
        Console.WriteLine("This is expected if the ID was just an example.");
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error resolving instrument IDs: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 3: Multiple Symbols Resolution
// ============================================================================

Console.WriteLine("Example 3: Resolve Multiple Symbols");
Console.WriteLine("------------------------------------");
Console.WriteLine("Resolve multiple symbols at once to see how they map over time.");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    string[] symbols = ["NVDA", "AAPL", "MSFT"];
    var startDate = new DateOnly(2024, 1, 1);
    var endDate = new DateOnly(2024, 2, 1);

    Console.WriteLine($"Dataset: {dataset}");
    Console.WriteLine($"Symbols: {string.Join(", ", symbols)}");
    Console.WriteLine($"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
    Console.WriteLine($"Converting: RawSymbol → InstrumentId");
    Console.WriteLine();

    var resolution = await client.SymbologyResolveAsync(
        dataset,
        symbols,
        SType.RawSymbol,
        SType.InstrumentId,
        startDate,
        endDate);

    Console.WriteLine($"Resolution Summary: {resolution}");
    Console.WriteLine();

    // Show summary statistics
    Console.WriteLine("Summary Statistics:");
    Console.WriteLine($"  Total symbols queried: {symbols.Length}");
    Console.WriteLine($"  Successfully mapped: {resolution.Mappings.Count}");
    Console.WriteLine($"  Partially mapped: {resolution.Partial.Count}");
    Console.WriteLine($"  Not found: {resolution.NotFound.Count}");
    Console.WriteLine();

    // Show all mappings
    if (resolution.Mappings.Count > 0)
    {
        Console.WriteLine("All Mappings:");
        foreach (var (inputSymbol, intervals) in resolution.Mappings.OrderBy(kvp => kvp.Key))
        {
            var firstInterval = intervals.FirstOrDefault();
            if (firstInterval != null)
            {
                Console.WriteLine($"  {inputSymbol,-6} → Instrument ID {firstInterval.Symbol}");
            }
        }
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error resolving multiple symbols: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Summary
// ============================================================================

Console.WriteLine("=== Symbology Resolution Summary ===");
Console.WriteLine();
Console.WriteLine("Symbology resolution allows you to:");
Console.WriteLine("  1. Convert between different symbol types (RawSymbol, InstrumentId, etc.)");
Console.WriteLine("  2. Track how symbols map over time (handle instrument changes, rollovers)");
Console.WriteLine("  3. Identify which symbols are valid for specific date ranges");
Console.WriteLine();
Console.WriteLine("Common use cases:");
Console.WriteLine("  - Convert ticker symbols to instrument IDs for data queries");
Console.WriteLine("  - Map futures symbols to continuous contracts");
Console.WriteLine("  - Validate symbol availability before making data requests");
Console.WriteLine();
Console.WriteLine("=== Symbology Example Complete ===");
