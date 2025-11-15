using Databento.Client.Builders;
using Databento.Client.Dbn;
using Databento.Client.Models;
using System.Reflection;

Console.WriteLine("=== DBN File Diagnostic Test ===\n");

var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
    ?? throw new InvalidOperationException("DATABENTO_API_KEY not set");

var client = new HistoricalClientBuilder()
    .WithApiKey(apiKey)
    .Build();

string sampleFilePath = Path.Combine(Path.GetTempPath(), "diagnostic_test.dbn.zst");

try
{
    // Download sample data
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

    Console.WriteLine($"✅ Downloaded: {sampleFilePath}\n");

    // Read and diagnose with DbnFileReader directly
    using var reader = new DbnFileReader(sampleFilePath);
    var metadata = reader.GetMetadata();

    Console.WriteLine("Metadata Timestamps:");
    Console.WriteLine($"  Start: {metadata.Start} ns = {DateTimeOffset.FromUnixTimeMilliseconds(metadata.Start / 1_000_000):O}");
    Console.WriteLine($"  End:   {metadata.End} ns = {DateTimeOffset.FromUnixTimeMilliseconds(metadata.End / 1_000_000):O}");
    Console.WriteLine();

    Console.WriteLine("First 3 Trade Records (RAW VALUES):");
    Console.WriteLine();

    // Use async reader to see diagnostic output
    int count = 0;
    await foreach (var record in reader.ReadRecordsAsync())
    {
        count++;
        if (count <= 3 && record is TradeMessage trade)
        {
            Console.WriteLine($"Trade #{count}:");
            Console.WriteLine($"  TimestampNs (raw): {trade.TimestampNs}");
            Console.WriteLine($"  Timestamp converted: {trade.Timestamp:O}");
            Console.WriteLine($"  Expected timestamp: ~{DateTimeOffset.FromUnixTimeMilliseconds(metadata.Start / 1_000_000):O}");
            Console.WriteLine($"  Price (raw int64): {trade.Price}");
            Console.WriteLine($"  Price hex: 0x{trade.Price:X16}");
            Console.WriteLine($"  PriceDecimal: {trade.PriceDecimal:F2}");
            Console.WriteLine($"  Expected price: ~$490");
            Console.WriteLine($"  Size: {trade.Size}");
            Console.WriteLine($"  InstrumentId: {trade.InstrumentId}");
            Console.WriteLine($"  RType: 0x{trade.RType:X2}");

            // Try to access RawBytes via reflection
            var rawBytesField = typeof(Record).GetField("RawBytes", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rawBytesField != null)
            {
                var rawBytes = rawBytesField.GetValue(trade) as byte[];
                if (rawBytes != null && rawBytes.Length >= 24)
                {
                    Console.WriteLine($"  RawBytes first 24 bytes (hex):");
                    Console.WriteLine($"    00-07 (header): {BitConverter.ToString(rawBytes, 0, 8)}");
                    Console.WriteLine($"    08-15 (ts_event): {BitConverter.ToString(rawBytes, 8, 8)}");
                    Console.WriteLine($"    16-23 (price): {BitConverter.ToString(rawBytes, 16, 8)}");

                    // Manually decode
                    long tsFromBytes = BitConverter.ToInt64(rawBytes, 8);
                    long priceFromBytes = BitConverter.ToInt64(rawBytes, 16);
                    Console.WriteLine($"  Manual decode from RawBytes:");
                    Console.WriteLine($"    ts_event: {tsFromBytes}");
                    Console.WriteLine($"    price: {priceFromBytes}");
                }
            }

            Console.WriteLine();
        }

        if (count >= 3) break;
    }

}
finally
{
    if (File.Exists(sampleFilePath))
    {
        File.Delete(sampleFilePath);
        Console.WriteLine("✅ Cleaned up diagnostic file");
    }
}
