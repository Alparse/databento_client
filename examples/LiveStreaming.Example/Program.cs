using Databento.Client.Builders;
using Databento.Client.Models;

namespace LiveStreaming.Example;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Databento Live Streaming Example");
        Console.WriteLine("=================================\n");
         
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
            // Create live client
            await using var client = new LiveClientBuilder()
                .WithApiKey(apiKey)
                .Build();

            Console.WriteLine("✓ Created live client");

            // Subscribe to data received events
            client.DataReceived += (sender, e) =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Received: {e.Record}");
            };

            // Subscribe to error events
            client.ErrorOccurred += (sender, e) =>
            {
                Console.WriteLine($"[ERROR] {e.Exception.Message}");
            };

            // Subscribe to equity trades on EQUS.MINI dataset
            await client.SubscribeAsync(
                dataset: "EQUS.MINI",
                schema: Schema.Trades,
                symbols: new[] { "QQQ" }
            );

            Console.WriteLine("✓ Subscribed to QQQ trades on EQUS.MINI");

            // Start streaming
            Console.WriteLine("✓ Starting stream...\n");
            var startTask = client.StartAsync();

            // Stream records using IAsyncEnumerable
            var count = 0;
            await foreach (var record in client.StreamAsync())
            {
                // Process records here
                count++;

                // Stop after 100 records for demo purposes
                if (count >= 10000000)
                {
                    Console.WriteLine($"\n✓ Received {count} records, stopping...");
                    break;
                }
            }

            // Stop streaming
            await client.StopAsync();
            Console.WriteLine("✓ Stopped stream");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
