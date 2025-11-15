using Databento.Client.Builders;
using Databento.Client.Models;

Console.WriteLine("=== Databento Size Limits Example ===");
Console.WriteLine();
Console.WriteLine("This example demonstrates how to manage request sizes and costs:");
Console.WriteLine("  - Check record count before making requests");
Console.WriteLine("  - Check billable size to estimate costs");
Console.WriteLine("  - Split large requests into smaller time ranges");
Console.WriteLine("  - Choose between stream and batch download");
Console.WriteLine();

var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
    ?? throw new InvalidOperationException(
        "DATABENTO_API_KEY environment variable is not set. " +
        "Set it with your API key to authenticate.");

var client = new HistoricalClientBuilder()
    .WithApiKey(apiKey)
    .Build();

// Example query parameters
string dataset = "EQUS.MINI";
string[] symbols = ["NVDA"];  // NVIDIA stock
Schema schema = Schema.Trades;
var startDate = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
var endDate = new DateTimeOffset(2024, 1, 31, 23, 59, 59, TimeSpan.Zero);

Console.WriteLine($"Query Parameters:");
Console.WriteLine($"  Dataset: {dataset}");
Console.WriteLine($"  Symbols: {string.Join(", ", symbols)}");
Console.WriteLine($"  Schema: {schema}");
Console.WriteLine($"  Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
Console.WriteLine();

// ============================================================================
// Example 1: Check record count before requesting data
// ============================================================================

Console.WriteLine("Example 1: Check Record Count");
Console.WriteLine("-------------------------------");

try
{
    var recordCount = await client.GetRecordCountAsync(
        dataset,
        schema,
        startDate,
        endDate,
        symbols);

    Console.WriteLine($"Record count for query: {recordCount:N0} records");
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting record count: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 2: Check billable size to estimate costs
// ============================================================================

Console.WriteLine("Example 2: Check Billable Size");
Console.WriteLine("--------------------------------");

try
{
    var billableSize = await client.GetBillableSizeAsync(
        dataset,
        schema,
        startDate,
        endDate,
        symbols);

    // Convert to human-readable format
    double sizeInMB = billableSize / (1024.0 * 1024.0);
    double sizeInGB = billableSize / (1024.0 * 1024.0 * 1024.0);

    Console.WriteLine($"Billable size: {billableSize:N0} bytes");
    if (sizeInGB >= 1.0)
    {
        Console.WriteLine($"             = {sizeInGB:N2} GB");
        Console.WriteLine();
        Console.WriteLine("Recommendation: This is a large dataset (>1 GB).");
        Console.WriteLine("Consider using batch download for better manageability.");
    }
    else
    {
        Console.WriteLine($"             = {sizeInMB:N2} MB");
        Console.WriteLine();
        Console.WriteLine("Recommendation: This dataset is small enough for streaming.");
    }
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting billable size: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 3: Get cost estimate
// ============================================================================

Console.WriteLine("Example 3: Get Cost Estimate");
Console.WriteLine("-----------------------------");

try
{
    var cost = await client.GetCostAsync(
        dataset,
        schema,
        startDate,
        endDate,
        symbols);

    Console.WriteLine($"Estimated cost: ${cost:F4} USD");
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting cost: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 4: Get combined billing information
// ============================================================================

Console.WriteLine("Example 4: Get Combined Billing Info");
Console.WriteLine("--------------------------------------");

try
{
    var billingInfo = await client.GetBillingInfoAsync(
        dataset,
        schema,
        startDate,
        endDate,
        symbols);

    Console.WriteLine($"Record count:  {billingInfo.RecordCount:N0} records");
    Console.WriteLine($"Billable size: {billingInfo.BillableSizeMB:N2} MB");
    Console.WriteLine($"Estimated cost: ${billingInfo.Cost:F4} USD");
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting billing info: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 5: Splitting large requests by time range
// ============================================================================

Console.WriteLine("Example 5: Splitting Large Requests");
Console.WriteLine("------------------------------------");
Console.WriteLine("For large datasets (>5 GB), split your request into smaller time ranges.");
Console.WriteLine();

// Split the month into weekly chunks
var weekStart = startDate;
int weekNumber = 1;

Console.WriteLine("Weekly breakdown for January 2024:");
Console.WriteLine();

while (weekStart < endDate)
{
    var weekEnd = weekStart.AddDays(7);
    if (weekEnd > endDate)
        weekEnd = endDate;

    try
    {
        var weeklyCount = await client.GetRecordCountAsync(
            dataset,
            schema,
            weekStart,
            weekEnd,
            symbols);

        var weeklySize = await client.GetBillableSizeAsync(
            dataset,
            schema,
            weekStart,
            weekEnd,
            symbols);

        Console.WriteLine($"Week {weekNumber}: {weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd}");
        Console.WriteLine($"  Records: {weeklyCount:N0}");
        Console.WriteLine($"  Size: {weeklySize / (1024.0 * 1024.0):N2} MB");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Week {weekNumber}: Error - {ex.Message}");
        Console.WriteLine();
    }

    weekStart = weekEnd;
    weekNumber++;
}

// ============================================================================
// Best Practices Summary
// ============================================================================

Console.WriteLine("=== Best Practices for Managing Request Sizes ===");
Console.WriteLine();
Console.WriteLine("1. CHECK FIRST:");
Console.WriteLine("   - Use GetRecordCountAsync() to check the number of records");
Console.WriteLine("   - Use GetBillableSizeAsync() to check the data size");
Console.WriteLine("   - Use GetCostAsync() to estimate costs before submitting");
Console.WriteLine();
Console.WriteLine("2. SIZE LIMITS:");
Console.WriteLine("   - No hard limit on request size");
Console.WriteLine("   - Batch download recommended for datasets >5 GB");
Console.WriteLine("   - Stream download works well for smaller datasets");
Console.WriteLine();
Console.WriteLine("3. SPLITTING STRATEGIES:");
Console.WriteLine("   - Split by time range (e.g., daily, weekly, monthly)");
Console.WriteLine("   - Split by symbol (request different symbols separately)");
Console.WriteLine("   - Use nanosecond-resolution time ranges for precise control");
Console.WriteLine();
Console.WriteLine("4. CHOOSING STREAM VS BATCH:");
Console.WriteLine("   - Stream: Real-time processing, smaller datasets (<5 GB)");
Console.WriteLine("   - Batch: Large datasets, background processing, file delivery");
Console.WriteLine();
Console.WriteLine("=== Size Limits Example Complete ===");
