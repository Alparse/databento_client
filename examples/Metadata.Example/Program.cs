using Databento.Client.Builders;
using Databento.Client.Models;

Console.WriteLine("=== Databento Metadata Example ===");
Console.WriteLine();
Console.WriteLine("This example demonstrates how to query metadata about");
Console.WriteLine("publishers, datasets, schemas, and fields.");
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
// Example 1: List All Publishers
// ============================================================================

Console.WriteLine("Example 1: List All Publishers");
Console.WriteLine("-------------------------------");
Console.WriteLine("Publishers are data providers. Each has an ID, dataset, venue, and description.");
Console.WriteLine();

try
{
    var publishers = await client.ListPublishersAsync();

    Console.WriteLine($"Found {publishers.Count} publishers:");
    Console.WriteLine();

    // Display first 10 publishers
    var count = 0;
    foreach (var publisher in publishers.Take(10))
    {
        count++;
        Console.WriteLine($"{count}. Publisher ID: {publisher.PublisherId}");
        Console.WriteLine($"   Dataset: {publisher.Dataset}");
        Console.WriteLine($"   Venue: {publisher.Venue}");
        Console.WriteLine($"   Description: {publisher.Description}");
        Console.WriteLine();
    }

    if (publishers.Count > 10)
    {
        Console.WriteLine($"... and {publishers.Count - 10} more publishers");
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing publishers: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 2: List All Datasets
// ============================================================================

Console.WriteLine("Example 2: List All Datasets");
Console.WriteLine("-----------------------------");
Console.WriteLine("Datasets represent different market data feeds from various exchanges.");
Console.WriteLine();

try
{
    var datasets = await client.ListDatasetsAsync();

    Console.WriteLine($"Found {datasets.Count} datasets:");
    Console.WriteLine();

    // Group datasets by exchange (first part before the dot)
    var groupedDatasets = datasets
        .GroupBy(d => d.Contains('.') ? d.Split('.')[0] : d)
        .OrderBy(g => g.Key);

    foreach (var group in groupedDatasets)
    {
        Console.WriteLine($"{group.Key}:");
        foreach (var dataset in group.OrderBy(d => d))
        {
            Console.Write($"  - {dataset}");

            // Add description for common datasets
            if (dataset == "EQUS.MINI")
                Console.Write(" (US Equities - Mini feed)");
            else if (dataset == "XNAS.ITCH")
                Console.Write(" (NASDAQ ITCH feed)");
            else if (dataset.StartsWith("GLBX."))
                Console.Write(" (CME Globex futures)");

            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing datasets: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 3: List Schemas for a Dataset
// ============================================================================

Console.WriteLine("Example 3: List Schemas for a Dataset");
Console.WriteLine("--------------------------------------");
Console.WriteLine("Schemas define the structure of market data (trades, quotes, book updates, etc.)");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    Console.WriteLine($"Listing schemas for dataset: {dataset}");
    Console.WriteLine();

    var schemas = await client.ListSchemasAsync(dataset);

    Console.WriteLine($"Found {schemas.Count} schemas:");
    foreach (var schema in schemas)
    {
        Console.Write($"  - {schema}");

        // Add descriptions
        switch (schema)
        {
            case Schema.Trades:
                Console.Write(" (Individual trade transactions)");
                break;
            case Schema.Mbp1:
                Console.Write(" (Market by price - top of book)");
                break;
            case Schema.Mbp10:
                Console.Write(" (Market by price - 10 levels)");
                break;
            case Schema.Tbbo:
                Console.Write(" (Top of book - best bid/offer)");
                break;
            case Schema.Ohlcv1D:
                Console.Write(" (Daily OHLCV bars)");
                break;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing schemas: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 4: List Fields for a Schema
// ============================================================================

Console.WriteLine("Example 4: List Fields for a Schema");
Console.WriteLine("------------------------------------");
Console.WriteLine("Fields define the individual data elements in each record.");
Console.WriteLine();

try
{
    var encoding = Encoding.Dbn;
    var schema = Schema.Trades;

    Console.WriteLine($"Listing fields for:");
    Console.WriteLine($"  Encoding: {encoding}");
    Console.WriteLine($"  Schema: {schema}");
    Console.WriteLine();

    var fields = await client.ListFieldsAsync(encoding, schema);

    Console.WriteLine($"Found {fields.Count} fields:");
    Console.WriteLine();

    foreach (var field in fields.Take(15))  // Show first 15 fields
    {
        Console.WriteLine($"  - {field.Name,-20} ({field.TypeName})");
    }

    if (fields.Count > 15)
    {
        Console.WriteLine($"  ... and {fields.Count - 15} more fields");
    }
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing fields: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 5: Get Dataset Condition
// ============================================================================

Console.WriteLine("Example 5: Get Dataset Availability Condition");
Console.WriteLine("----------------------------------------------");
Console.WriteLine("Shows the current status and availability of a dataset.");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    Console.WriteLine($"Checking condition for dataset: {dataset}");
    Console.WriteLine();

    var condition = await client.GetDatasetConditionAsync(dataset);

    Console.WriteLine($"Dataset: {condition.Dataset}");
    Console.WriteLine($"Condition: {condition.Condition}");
    if (condition.LastModified.HasValue)
    {
        Console.WriteLine($"Last Modified: {condition.LastModified.Value:yyyy-MM-dd}");
    }
    if (!string.IsNullOrEmpty(condition.Message))
    {
        Console.WriteLine($"Message: {condition.Message}");
    }
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error getting dataset condition: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 6: Get Dataset Date Range
// ============================================================================

Console.WriteLine("Example 6: Get Dataset Date Range");
Console.WriteLine("----------------------------------");
Console.WriteLine("Shows the available date range for a dataset.");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    Console.WriteLine($"Getting date range for dataset: {dataset}");
    Console.WriteLine();

    var range = await client.GetDatasetRangeAsync(dataset);

    Console.WriteLine($"Available Data Range:");
    Console.WriteLine($"  Start: {range.Start:yyyy-MM-dd}");
    Console.WriteLine($"  End:   {range.End:yyyy-MM-dd}");

    // Calculate duration
    var duration = (range.End - range.Start).TotalDays;
    var years = duration / 365.25;
    Console.WriteLine($"  Duration: ~{years:F1} years ({duration:N0} days)");
    Console.WriteLine();

    // Show per-schema ranges if available
    if (range.RangeBySchema != null && range.RangeBySchema.Count > 0)
    {
        Console.WriteLine("Per-Schema Availability:");
        foreach (var (schemaName, schemaRange) in range.RangeBySchema.OrderBy(kvp => kvp.Key))
        {
            Console.WriteLine($"  {schemaName,-15} {schemaRange.Start} to {schemaRange.End}");
        }
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error getting dataset range: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 7: List Unit Prices
// ============================================================================

Console.WriteLine("Example 7: List Unit Prices");
Console.WriteLine("----------------------------");
Console.WriteLine("Shows the unit prices per schema for each feed mode in US dollars per GB.");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    Console.WriteLine($"Listing unit prices for dataset: {dataset}");
    Console.WriteLine();

    var unitPrices = await client.ListUnitPricesAsync(dataset);

    Console.WriteLine($"Found {unitPrices.Count} pricing mode(s):");
    Console.WriteLine();

    foreach (var priceForMode in unitPrices)
    {
        Console.WriteLine($"Feed Mode: {priceForMode.Mode}");
        Console.WriteLine($"  Unit Prices (USD per GB):");

        foreach (var schemaPrice in priceForMode.UnitPrices.OrderBy(kvp => kvp.Key.ToString()))
        {
            Console.WriteLine($"    {schemaPrice.Key,-15} ${schemaPrice.Value:N4}");
        }
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing unit prices: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 8: Get Dataset Condition for Date Range
// ============================================================================

Console.WriteLine("Example 8: Get Dataset Condition for Date Range");
Console.WriteLine("------------------------------------------------");
Console.WriteLine("Shows the data quality and availability for each date in a specific range.");
Console.WriteLine();

try
{
    string dataset = "EQUS.MINI";
    var startDate = new DateTimeOffset(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);  // First trading day of 2024
    var endDate = new DateTimeOffset(2024, 1, 5, 0, 0, 0, TimeSpan.Zero);    // One week later

    Console.WriteLine($"Getting condition details for dataset: {dataset}");
    Console.WriteLine($"Date range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
    Console.WriteLine();

    var conditions = await client.GetDatasetConditionAsync(dataset, startDate, endDate);

    Console.WriteLine($"Found {conditions.Count} date(s) with condition information:");
    Console.WriteLine();

    foreach (var condition in conditions)
    {
        Console.Write($"  {condition.Date}: {condition.Condition}");

        if (!string.IsNullOrEmpty(condition.LastModifiedDate))
        {
            Console.Write($" (last modified: {condition.LastModifiedDate})");
        }

        Console.WriteLine();
    }
    Console.WriteLine();

    // Show summary statistics
    var conditionGroups = conditions.GroupBy(c => c.Condition)
                                   .OrderByDescending(g => g.Count());

    Console.WriteLine("Condition Summary:");
    foreach (var group in conditionGroups)
    {
        Console.WriteLine($"  {group.Key}: {group.Count()} day(s)");
    }
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error getting dataset condition: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Summary
// ============================================================================

Console.WriteLine("=== Metadata Query Methods Summary ===");
Console.WriteLine();
Console.WriteLine("The Historical client provides these metadata methods:");
Console.WriteLine();
Console.WriteLine("1. ListPublishersAsync()");
Console.WriteLine("   - Returns all publisher mappings (ID, dataset, venue, description)");
Console.WriteLine();
Console.WriteLine("2. ListDatasetsAsync(venue?)");
Console.WriteLine("   - Returns available datasets, optionally filtered by venue");
Console.WriteLine();
Console.WriteLine("3. ListSchemasAsync(dataset)");
Console.WriteLine("   - Returns available schemas for a specific dataset");
Console.WriteLine();
Console.WriteLine("4. ListFieldsAsync(encoding, schema)");
Console.WriteLine("   - Returns field definitions for a schema");
Console.WriteLine();
Console.WriteLine("5. GetDatasetConditionAsync(dataset)");
Console.WriteLine("   - Returns availability status of a dataset");
Console.WriteLine();
Console.WriteLine("6. GetDatasetRangeAsync(dataset)");
Console.WriteLine("   - Returns the date range of available data");
Console.WriteLine();
Console.WriteLine("7. ListUnitPricesAsync(dataset)");
Console.WriteLine("   - Returns unit prices per schema for each feed mode in USD/GB");
Console.WriteLine();
Console.WriteLine("8. GetDatasetConditionAsync(dataset, startDate, endDate?)");
Console.WriteLine("   - Returns per-date condition details for a specific date range");
Console.WriteLine();
Console.WriteLine("9. GetRecordCountAsync(...) - See SizeLimits.Example");
Console.WriteLine("10. GetBillableSizeAsync(...) - See SizeLimits.Example");
Console.WriteLine("11. GetCostAsync(...) - See SizeLimits.Example");
Console.WriteLine();

Console.WriteLine("=== Metadata Example Complete ===");
