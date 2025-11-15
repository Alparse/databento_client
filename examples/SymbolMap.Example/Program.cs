using Databento.Client.Builders;
using Databento.Client.Dbn;
using Databento.Client.Metadata;
using Databento.Client.Models;

Console.WriteLine("=== Databento Symbol Mapping Example ===");
Console.WriteLine();
Console.WriteLine("This example demonstrates how to create and use symbol maps");
Console.WriteLine("to resolve instrument IDs to symbols across time periods.");
Console.WriteLine();

var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
    ?? throw new InvalidOperationException(
        "DATABENTO_API_KEY environment variable is not set. " +
        "Set it with your API key to authenticate.");

// ============================================================================
// Setup: Download Sample Data with Symbol Mappings
// ============================================================================

Console.WriteLine("Setup: Downloading Sample Data");
Console.WriteLine("-------------------------------");
Console.WriteLine("Requesting historical data that includes symbol mappings.");
Console.WriteLine();

var client = new HistoricalClientBuilder()
    .WithApiKey(apiKey)
    .Build();

string sampleFilePath = Path.Combine(Path.GetTempPath(), "databento_symbolmap_sample.dbn.zst");

try
{
    // Request data that will include symbol mappings
    var startTime = new DateTimeOffset(2024, 1, 2, 12, 0, 0, TimeSpan.Zero);
    var endTime = new DateTimeOffset(2024, 1, 2, 12, 5, 0, TimeSpan.Zero);

    Console.WriteLine("  Dataset: EQUS.MINI");
    Console.WriteLine("  Symbols: NVDA, AAPL");
    Console.WriteLine("  Schema: Trades");
    Console.WriteLine("  Date: 2024-01-02, 12:00-12:05");
    Console.WriteLine();

    var filePath = await client.GetRangeToFileAsync(
        sampleFilePath,
        "EQUS.MINI",
        Schema.Trades,
        new[] { "NVDA", "AAPL" },
        startTime,
        endTime);

    Console.WriteLine($"✅ Downloaded sample file");
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error downloading data: {ex.Message}");
    return;
}

// ============================================================================
// Example 1: Access Symbol Mappings from DBN Metadata
// ============================================================================

Console.WriteLine("Example 1: Read Symbol Mappings from DBN File");
Console.WriteLine("----------------------------------------------");
Console.WriteLine("DBN files contain metadata with symbol mapping information.");
Console.WriteLine();

Databento.Client.Models.Dbn.DbnMetadata? fileMetadata = null;
var instrumentIds = new HashSet<uint>();

try
{
    using var store = new DbnFileStore(sampleFilePath);
    fileMetadata = store.Metadata;

    Console.WriteLine("File Metadata:");
    Console.WriteLine($"  Dataset: {fileMetadata.Dataset}");
    Console.WriteLine($"  Symbols: {string.Join(", ", fileMetadata.Symbols)}");
    Console.WriteLine($"  Stype Out: {fileMetadata.StypeOut}");
    Console.WriteLine();

    // Collect instrument IDs from records
    store.Replay(record =>
    {
        instrumentIds.Add(record.InstrumentId);
        return instrumentIds.Count < 10; // Just collect first 10 unique IDs
    });

    Console.WriteLine($"Found {instrumentIds.Count} unique instrument ID(s) in first records:");
    foreach (var id in instrumentIds.OrderBy(x => x))
    {
        Console.WriteLine($"  - Instrument ID: {id}");
    }
    Console.WriteLine();

    // Show symbol mappings if available
    if (fileMetadata.Mappings.Count > 0)
    {
        Console.WriteLine($"Symbol Mappings ({fileMetadata.Mappings.Count}):");
        foreach (var mapping in fileMetadata.Mappings)
        {
            Console.WriteLine($"  {mapping.RawSymbol}:");
            foreach (var interval in mapping.Intervals)
            {
                Console.WriteLine($"    {interval.StartDate} to {interval.EndDate}: {interval.Symbol}");
            }
        }
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("Note: This file does not contain symbol mappings.");
        Console.WriteLine("Symbol mappings are included when stype_in != stype_out");
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error reading metadata: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 2: Demonstrate Record-Based Convenience Overloads
// ============================================================================

Console.WriteLine("Example 2: Record-Based Convenience Methods");
Console.WriteLine("--------------------------------------------");
Console.WriteLine("TsSymbolMap and PitSymbolMap provide convenient overloads");
Console.WriteLine("that accept Record objects directly.");
Console.WriteLine();

try
{
    using var store = new DbnFileStore(sampleFilePath);

    Console.WriteLine("API Comparison:");
    Console.WriteLine();

    Console.WriteLine("Traditional API (manual extraction):");
    Console.WriteLine("  var date = DateOnly.FromDateTime(record.Timestamp.DateTime);");
    Console.WriteLine("  var symbol = tsMap.Find(date, record.InstrumentId);");
    Console.WriteLine();

    Console.WriteLine("Convenience API (automatic extraction):");
    Console.WriteLine("  var symbol = tsMap.Find(record);  // Extracts date & ID");
    Console.WriteLine("  var symbol = tsMap.At(record);    // Throws if not found");
    Console.WriteLine();

    Console.WriteLine("  var symbol = pitMap.Find(record); // Extracts ID only");
    Console.WriteLine("  var symbol = pitMap.At(record);   // Throws if not found");
    Console.WriteLine();

    Console.WriteLine("Processing sample records:");
    Console.WriteLine();

    int recordCount = 0;
    store.Replay(record =>
    {
        recordCount++;
        if (recordCount <= 3)
        {
            var date = DateOnly.FromDateTime(record.Timestamp.DateTime);
            Console.WriteLine($"  Record #{recordCount}:");
            Console.WriteLine($"    Date: {date}, Instrument ID: {record.InstrumentId}");
            Console.WriteLine($"    Would use: tsMap.At(record) or pitMap.At(record)");
            Console.WriteLine();
        }
        return recordCount < 3;
    });

    Console.WriteLine("Benefits:");
    Console.WriteLine("  ✓ Less boilerplate code in record loops");
    Console.WriteLine("  ✓ Reduced chance of extraction errors");
    Console.WriteLine("  ✓ Cleaner, more readable code");
    Console.WriteLine("  ✓ Matches C++ databento API patterns");
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 3: Metadata.CreateSymbolMap() for Streaming Operations
// ============================================================================

Console.WriteLine("Example 3: Metadata.CreateSymbolMap() Pattern");
Console.WriteLine("----------------------------------------------");
Console.WriteLine("During streaming operations (historical or live), the library");
Console.WriteLine("provides Metadata objects via callbacks. These can create");
Console.WriteLine("efficient symbol map indexes for lookups.");
Console.WriteLine();

Console.WriteLine("Typical usage pattern with Historical Streaming:");
Console.WriteLine();
Console.WriteLine("```csharp");
Console.WriteLine("await client.Timeseries.StreamAsync(");
Console.WriteLine("    dataset: \"EQUS.MINI\",");
Console.WriteLine("    symbols: new[] { \"NVDA\" },");
Console.WriteLine("    schema: Schema.Trades,");
Console.WriteLine("    start: startTime,");
Console.WriteLine("    end: endTime,");
Console.WriteLine("    metadataCallback: (Metadata metadata) =>");
Console.WriteLine("    {");
Console.WriteLine("        // Create a timeseries symbol map (date + instrument ID -> symbol)");
Console.WriteLine("        using var tsMap = metadata.CreateSymbolMap();");
Console.WriteLine("        ");
Console.WriteLine("        Console.WriteLine($\"Symbol map size: {tsMap.Size}\");");
Console.WriteLine("        Console.WriteLine($\"Is empty: {tsMap.IsEmpty}\");");
Console.WriteLine("        ");
Console.WriteLine("        // Look up symbol for specific date and instrument ID");
Console.WriteLine("        var symbol = tsMap.Find(new DateOnly(2024, 1, 2), instrumentId: 11667);");
Console.WriteLine("        Console.WriteLine($\"Instrument 11667 on 2024-01-02: {symbol}\");");
Console.WriteLine("        ");
Console.WriteLine("        // Or use At() which throws if not found");
Console.WriteLine("        try");
Console.WriteLine("        {");
Console.WriteLine("            symbol = tsMap.At(new DateOnly(2024, 1, 2), instrumentId: 11667);");
Console.WriteLine("        }");
Console.WriteLine("        catch (KeyNotFoundException)");
Console.WriteLine("        {");
Console.WriteLine("            Console.WriteLine(\"Symbol not found\");");
Console.WriteLine("        }");
Console.WriteLine("    },");
Console.WriteLine("    recordCallback: (Record record) =>");
Console.WriteLine("    {");
Console.WriteLine("        // Process records...");
Console.WriteLine("        return true;");
Console.WriteLine("    });");
Console.WriteLine("```");
Console.WriteLine();

// ============================================================================
// Example 3: Metadata.CreateSymbolMapForDate() for Point-in-Time Lookups
// ============================================================================

Console.WriteLine("Example 3: Metadata.CreateSymbolMapForDate() Pattern");
Console.WriteLine("----------------------------------------------------");
Console.WriteLine("For queries over a single trading day, you can create a");
Console.WriteLine("point-in-time symbol map that doesn't require date lookups.");
Console.WriteLine();

Console.WriteLine("Typical usage pattern:");
Console.WriteLine();
Console.WriteLine("```csharp");
Console.WriteLine("await client.Timeseries.StreamAsync(");
Console.WriteLine("    dataset: \"EQUS.MINI\",");
Console.WriteLine("    symbols: new[] { \"NVDA\", \"AAPL\" },");
Console.WriteLine("    schema: Schema.Trades,");
Console.WriteLine("    start: startTime,");
Console.WriteLine("    end: endTime,");
Console.WriteLine("    metadataCallback: (Metadata metadata) =>");
Console.WriteLine("    {");
Console.WriteLine("        // Create a point-in-time map for a specific date");
Console.WriteLine("        using var pitMap = metadata.CreateSymbolMapForDate(new DateOnly(2024, 1, 2));");
Console.WriteLine("        ");
Console.WriteLine("        Console.WriteLine($\"PIT Symbol map size: {pitMap.Size}\");");
Console.WriteLine("        Console.WriteLine($\"Is empty: {pitMap.IsEmpty}\");");
Console.WriteLine("        ");
Console.WriteLine("        // Look up symbol by instrument ID (no date needed)");
Console.WriteLine("        var symbol = pitMap.Find(instrumentId: 11667);");
Console.WriteLine("        Console.WriteLine($\"Instrument 11667: {symbol}\");");
Console.WriteLine("        ");
Console.WriteLine("        // Or use At() which throws if not found");
Console.WriteLine("        try");
Console.WriteLine("        {");
Console.WriteLine("            symbol = pitMap.At(instrumentId: 11667);");
Console.WriteLine("        }");
Console.WriteLine("        catch (KeyNotFoundException)");
Console.WriteLine("        {");
Console.WriteLine("            Console.WriteLine(\"Symbol not found\");");
Console.WriteLine("        }");
Console.WriteLine("    },");
Console.WriteLine("    recordCallback: (Record record) =>");
Console.WriteLine("    {");
Console.WriteLine("        // Process records...");
Console.WriteLine("        return true;");
Console.WriteLine("    });");
Console.WriteLine("```");
Console.WriteLine();

// ============================================================================
// Example 4: PitSymbolMap with Live Data
// ============================================================================

Console.WriteLine("Example 4: PitSymbolMap Update Methods for Live Streaming");
Console.WriteLine("-----------------------------------------------------------");
Console.WriteLine("During live streaming, symbol mappings arrive as special");
Console.WriteLine("SymbolMapping records. PitSymbolMap provides two methods to");
Console.WriteLine("update from these records.");
Console.WriteLine();

Console.WriteLine("Pattern 1: OnRecord() - Generic method (accepts any record):");
Console.WriteLine();
Console.WriteLine("```csharp");
Console.WriteLine("liveClient.DataReceived += (sender, e) =>");
Console.WriteLine("{");
Console.WriteLine("    var record = e.Record;");
Console.WriteLine("    ");
Console.WriteLine("    // OnRecord() silently ignores non-SymbolMapping records");
Console.WriteLine("    pitMap.OnRecord(record);  // Only RType 0x1B affects the map");
Console.WriteLine("    ");
Console.WriteLine("    // Use the map to resolve instrument IDs");
Console.WriteLine("    var symbol = pitMap.Find(record.InstrumentId);");
Console.WriteLine("    Console.WriteLine($\"Record for {symbol ?? \"unknown\"}\");");
Console.WriteLine("};");
Console.WriteLine("```");
Console.WriteLine();

Console.WriteLine("Pattern 2: OnSymbolMapping() - Type-safe method (only SymbolMappingMessage):");
Console.WriteLine();
Console.WriteLine("```csharp");
Console.WriteLine("liveClient.DataReceived += (sender, e) =>");
Console.WriteLine("{");
Console.WriteLine("    var record = e.Record;");
Console.WriteLine("    ");
Console.WriteLine("    // Type-check and cast for compile-time safety");
Console.WriteLine("    if (record is SymbolMappingMessage symbolMapping)");
Console.WriteLine("    {");
Console.WriteLine("        pitMap.OnSymbolMapping(symbolMapping);  // Type-safe!");
Console.WriteLine("        Console.WriteLine($\"Added mapping: {symbolMapping.STypeInSymbol}\");");
Console.WriteLine("        Console.WriteLine($\"  {symbolMapping.STypeIn} -> {symbolMapping.STypeOut}\");");
Console.WriteLine("        Console.WriteLine($\"  Symbol map size: {pitMap.Size}\");");
Console.WriteLine("    }");
Console.WriteLine("    ");
Console.WriteLine("    // Process data records");
Console.WriteLine("    var symbol = pitMap.Find(record.InstrumentId);");
Console.WriteLine("    Console.WriteLine($\"Record for {symbol ?? \"unknown\"}\");");
Console.WriteLine("};");
Console.WriteLine("```");
Console.WriteLine();

Console.WriteLine("Key Differences:");
Console.WriteLine("  • OnRecord(Record): Generic, works with any record type");
Console.WriteLine("  • OnSymbolMapping(SymbolMappingMessage): Type-safe, compile-time checked");
Console.WriteLine("  • Both silently ignore/filter non-SymbolMapping records");
Console.WriteLine("  • OnSymbolMapping provides access to symbol mapping fields");
Console.WriteLine();

Console.WriteLine("Full example:");
Console.WriteLine();
Console.WriteLine("```csharp");
Console.WriteLine("var liveClient = new LiveClientBuilder()");
Console.WriteLine("    .WithApiKey(apiKey)");
Console.WriteLine("    .Build();");
Console.WriteLine();
Console.WriteLine("var pitMap = new PitSymbolMap();");
Console.WriteLine();
Console.WriteLine("liveClient.DataReceived += (sender, e) =>");
Console.WriteLine("{");
Console.WriteLine("    if (e.Record is SymbolMappingMessage symbolMapping)");
Console.WriteLine("    {");
Console.WriteLine("        pitMap.OnSymbolMapping(symbolMapping);");
Console.WriteLine("    }");
Console.WriteLine("    else");
Console.WriteLine("    {");
Console.WriteLine("        var symbol = pitMap.Find(e.Record);  // Convenience overload");
Console.WriteLine("        Console.WriteLine($\"Data for {symbol ?? e.Record.InstrumentId.ToString()}\");");
Console.WriteLine("    }");
Console.WriteLine("};");
Console.WriteLine();
Console.WriteLine("await liveClient.SubscribeAsync(\"EQUS.MINI\", [\"NVDA\"], Schema.Trades);");
Console.WriteLine("await liveClient.StartAsync();");
Console.WriteLine("```");
Console.WriteLine();

// ============================================================================
// Cleanup
// ============================================================================

Console.WriteLine("Cleanup");
Console.WriteLine("-------");
try
{
    if (File.Exists(sampleFilePath))
    {
        File.Delete(sampleFilePath);
        Console.WriteLine($"✅ Deleted sample file");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  Could not delete sample file: {ex.Message}");
}
Console.WriteLine();

// ============================================================================
// Summary
// ============================================================================

Console.WriteLine("=== Symbol Mapping Summary ===");
Console.WriteLine();
Console.WriteLine("Three types of symbol mapping interfaces:");
Console.WriteLine();
Console.WriteLine("1. DbnMetadata.Mappings (IReadOnlyList<SymbolMapping>)");
Console.WriteLine("   - Available from: DBN files (DbnFileStore.Metadata)");
Console.WriteLine("   - Use case: Reading symbol mappings from historical files");
Console.WriteLine("   - Access: Direct iteration over mappings and intervals");
Console.WriteLine();
Console.WriteLine("2. TsSymbolMap (Timeseries Symbol Map)");
Console.WriteLine("   - Available from: Metadata.CreateSymbolMap()");
Console.WriteLine("   - Use case: Efficient lookups across time ranges");
Console.WriteLine("   - Methods: Find(date, instrumentId), At(date, instrumentId)");
Console.WriteLine("   - When: Historical/live streaming with date-varying symbols");
Console.WriteLine();
Console.WriteLine("3. PitSymbolMap (Point-in-Time Symbol Map)");
Console.WriteLine("   - Available from: Metadata.CreateSymbolMapForDate(date)");
Console.WriteLine("   - Use case: Efficient lookups for single trading day");
Console.WriteLine("   - Methods: Find(instrumentId), At(instrumentId), OnRecord(record)");
Console.WriteLine("   - When: Single-day queries or live streaming");
Console.WriteLine();
Console.WriteLine("Key Differences:");
Console.WriteLine();
Console.WriteLine("TsSymbolMap vs PitSymbolMap:");
Console.WriteLine("  • TsSymbolMap: Requires date + instrument ID (spans multiple dates)");
Console.WriteLine("  • PitSymbolMap: Only needs instrument ID (single date snapshot)");
Console.WriteLine();
Console.WriteLine("DbnMetadata vs Metadata:");
Console.WriteLine("  • DbnMetadata: File/stream metadata with raw mapping data");
Console.WriteLine("  • Metadata: Runtime object for creating optimized lookup indexes");
Console.WriteLine();
Console.WriteLine("Best Practices:");
Console.WriteLine("  • Use DbnMetadata.Mappings for file-based analysis");
Console.WriteLine("  • Use TsSymbolMap for multi-day historical streaming");
Console.WriteLine("  • Use PitSymbolMap for single-day queries or live streaming");
Console.WriteLine("  • Always dispose symbol maps (use 'using' statements)");
Console.WriteLine("  • Call PitSymbolMap.OnRecord() for incremental updates in live streams");
Console.WriteLine();

Console.WriteLine("=== Symbol Mapping Example Complete ===");
