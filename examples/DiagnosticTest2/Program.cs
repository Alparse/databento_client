using Databento.Client.Builders;
using Databento.Client.Dbn;
using Databento.Client.Models;

Console.WriteLine("=== Test WITHOUT calling GetMetadata() first ===\n");

var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
    ?? throw new InvalidOperationException("DATABENTO_API_KEY not set");

var client = new HistoricalClientBuilder()
    .WithApiKey(apiKey)
    .Build();

string sampleFilePath = Path.Combine(Path.GetTempPath(), "diagnostic_test2.dbn.zst");

try
{
    Console.WriteLine("Downloading sample data...");
    var startTime = new DateTimeOffset(2024, 1, 2, 12, 0, 0, TimeSpan.Zero);
    var endTime = new DateTimeOffset(2024, 1, 2, 12, 5, 0, TimeSpan.Zero);

    await client.GetRangeToFileAsync(
        sampleFilePath,
        "EQUS.MINI",
        Schema.Trades,
        new[] { "NVDA" },
        startTime,
        endTime);

    Console.WriteLine($"✅ Downloaded\n");

    // Test 1: Read WITHOUT calling GetMetadata() first
    Console.WriteLine("Test 1: Reading records WITHOUT GetMetadata() call first");
    Console.WriteLine("--------------------------------------------------------");
    using var reader1 = new DbnFileReader(sampleFilePath);
    // DON'T call GetMetadata()!

    int count1 = 0;
    await foreach (var record in reader1.ReadRecordsAsync())
    {
        count1++;
        if (count1 == 1 && record is TradeMessage trade)
        {
            Console.WriteLine($"First trade (no metadata call):");
            Console.WriteLine($"  TimestampNs: {trade.TimestampNs}");
            Console.WriteLine($"  Price: {trade.Price} (0x{trade.Price:X16})");
            Console.WriteLine($"  InstrumentId: {trade.InstrumentId}");
            break;
        }
    }
    Console.WriteLine();

    // Test 2: Read WITH calling GetMetadata() first
    Console.WriteLine("Test 2: Reading records WITH GetMetadata() call first");
    Console.WriteLine("-----------------------------------------------------");
    using var reader2 = new DbnFileReader(sampleFilePath);
    var metadata = reader2.GetMetadata(); // Call GetMetadata()!
    Console.WriteLine($"Got metadata: {metadata.Dataset}");

    int count2 = 0;
    await foreach (var record in reader2.ReadRecordsAsync())
    {
        count2++;
        if (count2 == 1 && record is TradeMessage trade)
        {
            Console.WriteLine($"First trade (with metadata call):");
            Console.WriteLine($"  TimestampNs: {trade.TimestampNs}");
            Console.WriteLine($"  Price: {trade.Price} (0x{trade.Price:X16})");
            Console.WriteLine($"  InstrumentId: {trade.InstrumentId}");
            break;
        }
    }
    Console.WriteLine();

    if (count1 > 0 && count2 > 0)
    {
        Console.WriteLine("Hypothesis: GetMetadata() messes up file position for reading records!");
    }
}
finally
{
    if (File.Exists(sampleFilePath))
    {
        File.Delete(sampleFilePath);
        Console.WriteLine("✅ Cleaned up");
    }
}
