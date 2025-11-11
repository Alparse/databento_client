using Databento.Client.Builders;
using Databento.Client.Models;

namespace Advanced.Example;

/// <summary>
/// Advanced example demonstrating multiple schemas, record type handling, and features
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Databento.NET Advanced Example");
        Console.WriteLine("==============================\n");

        // Get API key from environment variable or command line
        var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
                     ?? (args.Length > 0 ? args[0] : null);

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Error: API key not provided.");
            Console.WriteLine("Usage: dotnet run <api-key>");
            Console.WriteLine("Or set DATABENTO_API_KEY environment variable");
            return;
        }

        try
        {
            // Demonstrate live streaming with multiple record types
            Console.WriteLine("=== Live Streaming with Multiple Schemas ===\n");
            await DemonstrateLiveStreaming(apiKey);

            Console.WriteLine("\n\n=== Historical Data Queries ===\n");
            await DemonstrateHistoricalQueries(apiKey);

            Console.WriteLine("\n\n=== Record Type Handling ===\n");
            await DemonstrateRecordTypeHandling(apiKey);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static async Task DemonstrateLiveStreaming(string apiKey)
    {
        await using var client = new LiveClientBuilder()
            .WithApiKey(apiKey)
            .Build();

        Console.WriteLine("✓ Created live client");

        // Subscribe to Market By Price Level 1 (best bid/offer)
        await client.SubscribeAsync(
            dataset: "EQUS.MINI",
            schema: Schema.Mbp1,
            symbols: new[] { "QQQ" }
        );

        Console.WriteLine("✓ Subscribed to QQQ MBP-1 (best bid/offer)");
        Console.WriteLine("✓ Starting stream (will show first 20 records)...\n");

        var startTask = client.StartAsync();

        var count = 0;
        await foreach (var record in client.StreamAsync())
        {
            count++;

            if (count <= 20)
            {
                // Demonstrate type-specific handling
                switch (record)
                {
                    case Mbp1Message mbp:
                        Console.WriteLine($"[{count:D3}] MBP1: Bid ${Constants.PriceToDecimal(mbp.Level.BidPrice):F2} x {mbp.Level.BidSize}, " +
                                        $"Ask ${Constants.PriceToDecimal(mbp.Level.AskPrice):F2} x {mbp.Level.AskSize}");
                        break;
                    case TradeMessage trade:
                        Console.WriteLine($"[{count:D3}] Trade: {trade.Size} @ ${trade.PriceDecimal:F2} (Side: {trade.Side})");
                        break;
                    default:
                        Console.WriteLine($"[{count:D3}] {record.GetType().Name}: {record}");
                        break;
                }
            }
            else if (count == 21)
            {
                Console.WriteLine("...");
            }

            // Stop after demonstrating
            if (count >= 100)
            {
                Console.WriteLine($"\n✓ Received {count} records");
                break;
            }
        }

        await client.StopAsync();
        Console.WriteLine("✓ Stopped stream");
    }

    static async Task DemonstrateHistoricalQueries(string apiKey)
    {
        await using var client = new HistoricalClientBuilder()
            .WithApiKey(apiKey)
            .Build();

        Console.WriteLine("✓ Created historical client");

        // Query last hour of trade data
        var endTime = DateTimeOffset.UtcNow;
        var startTime = endTime.AddHours(-1);

        Console.WriteLine($"✓ Querying trades from {startTime:HH:mm:ss} to {endTime:HH:mm:ss}");
        Console.WriteLine("✓ Symbol: ES.FUT (E-mini S&P 500 Futures)\n");

        var count = 0;
        decimal totalVolume = 0;
        decimal volumeWeightedPrice = 0;

        await foreach (var record in client.GetRangeAsync(
            dataset: "GLBX.MDP3",
            schema: Schema.Trades,
            symbols: new[] { "ES.FUT" },
            startTime: startTime,
            endTime: endTime))
        {
            if (record is TradeMessage trade)
            {
                count++;
                totalVolume += trade.Size;
                volumeWeightedPrice += trade.PriceDecimal * trade.Size;

                // Print first few
                if (count <= 5)
                {
                    Console.WriteLine($"[{count}] Trade: {trade.Size} @ ${trade.PriceDecimal:F2}");
                }
            }

            // Limit for demo
            if (count >= 1000)
            {
                break;
            }
        }

        if (count > 0 && totalVolume > 0)
        {
            var vwap = volumeWeightedPrice / totalVolume;
            Console.WriteLine($"\n✓ Processed {count:N0} trades");
            Console.WriteLine($"✓ Total volume: {totalVolume:N0}");
            Console.WriteLine($"✓ VWAP: ${vwap:F2}");
        }
        else
        {
            Console.WriteLine("\n⚠ No trades found in time range (markets may be closed)");
        }
    }

    static async Task DemonstrateRecordTypeHandling(string apiKey)
    {
        Console.WriteLine("Databento.NET supports all 16 DBN record types:\n");

        Console.WriteLine("Market Data Record Types:");
        Console.WriteLine("  • TradeMessage (RType 0x00) - Trades");
        Console.WriteLine("  • MboMessage (RType 0xA0) - Market by Order");
        Console.WriteLine("  • Mbp1Message (RType 0x01) - Market by Price Level 1");
        Console.WriteLine("  • Mbp10Message (RType 0x02) - Market by Price Level 10");
        Console.WriteLine("  • BboMessage (RType 0xC2-0xC3) - Best Bid/Offer");
        Console.WriteLine("  • CbboMessage (RType 0xB2-0xB4) - Consolidated BBO");
        Console.WriteLine("  • Cmbp1Message (RType 0xB1) - Consolidated MBP Level 1");

        Console.WriteLine("\nBar/OHLCV Record Types:");
        Console.WriteLine("  • OhlcvMessage (RType 0x12-0x16) - OHLCV bars (1s, 1m, 1h, 1d, EOD)");

        Console.WriteLine("\nMetadata Record Types:");
        Console.WriteLine("  • InstrumentDefMessage (RType 0x18) - Instrument definitions");
        Console.WriteLine("  • StatusMessage (RType 0x17) - Trading status");
        Console.WriteLine("  • SymbolMappingMessage (RType 0x1B) - Symbol mappings");

        Console.WriteLine("\nOther Record Types:");
        Console.WriteLine("  • ImbalanceMessage (RType 0x19) - Order imbalances");
        Console.WriteLine("  • StatMessage (RType 0x1D) - Market statistics");
        Console.WriteLine("  • ErrorMessage (RType 0x1A) - Error messages");
        Console.WriteLine("  • SystemMessage (RType 0x1C) - System messages & heartbeats");
        Console.WriteLine("  • UnknownRecord - Fallback for unrecognized types");

        Console.WriteLine("\nHelper Classes:");
        Console.WriteLine("  • FlagSet - Bit flags (Last, Tob, Snapshot, Mbp, BadTsRecv, etc.)");
        Console.WriteLine("  • Constants - Fixed-point conversions, sentinel values");
        Console.WriteLine("  • Schema enums - All 13 schema types");

        Console.WriteLine("\nExample record type handling:");

        await using var client = new LiveClientBuilder()
            .WithApiKey(apiKey)
            .Build();

        await client.SubscribeAsync(
            dataset: "EQUS.MINI",
            schema: Schema.Trades,
            symbols: new[] { "QQQ" }
        );

        var startTask = client.StartAsync();

        var recordTypesSeen = new HashSet<string>();
        var count = 0;

        await foreach (var record in client.StreamAsync())
        {
            var typeName = record.GetType().Name;

            if (recordTypesSeen.Add(typeName))
            {
                Console.WriteLine($"  ✓ Received {typeName}: {record}");
            }

            count++;
            if (count >= 50 || recordTypesSeen.Count >= 3)
            {
                break;
            }
        }

        await client.StopAsync();

        Console.WriteLine($"\n✓ Demonstrated handling {recordTypesSeen.Count} record types in {count} records");
    }
}
