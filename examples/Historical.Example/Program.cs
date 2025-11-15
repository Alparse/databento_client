using Databento.Client.Builders;
using Databento.Client.Models;

Console.WriteLine("=== Databento Historical Client Example ===");
Console.WriteLine();
Console.WriteLine("This example demonstrates different ways to create and configure");
Console.WriteLine("the Historical client for accessing Databento's historical API.");
Console.WriteLine();

// ============================================================================
// Example 1: Using Environment Variable (RECOMMENDED for production)
// ============================================================================

Console.WriteLine("Example 1: Creating Client from Environment Variable");
Console.WriteLine("------------------------------------------------------");
Console.WriteLine("This is the RECOMMENDED approach for production applications.");
Console.WriteLine("Set the DATABENTO_API_KEY environment variable with your API key.");
Console.WriteLine();

try
{
    // Check if environment variable is set
    var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY");

    if (string.IsNullOrEmpty(apiKey))
    {
        Console.WriteLine("❌ DATABENTO_API_KEY environment variable is not set.");
        Console.WriteLine("   Set it with: $env:DATABENTO_API_KEY=\"your-api-key-here\"");
        Console.WriteLine();
    }
    else
    {
        // Create client using environment variable (recommended)
        var client = new HistoricalClientBuilder()
            .WithApiKey(apiKey)  // Reads from environment variable
            .Build();

        Console.WriteLine("✅ Successfully created Historical client from environment variable");
        Console.WriteLine($"   API Key: {apiKey.Substring(0, 8)}... (masked)");
        Console.WriteLine();

        // Verify client works by listing datasets
        Console.WriteLine("   Testing connection by listing datasets...");
        var datasets = await client.ListDatasetsAsync();
        Console.WriteLine($"   ✅ Connection successful! Found {datasets.Count} datasets");
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error creating client from environment: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 2: Passing API Key Directly (NOT recommended for production)
// ============================================================================

Console.WriteLine("Example 2: Creating Client with API Key Argument");
Console.WriteLine("-------------------------------------------------");
Console.WriteLine("NOT RECOMMENDED for production - API keys should not be hardcoded.");
Console.WriteLine("This approach is acceptable for testing and development only.");
Console.WriteLine();

try
{
    // For demonstration purposes - DO NOT hardcode keys in production!
    var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY");

    if (!string.IsNullOrEmpty(apiKey))
    {
        // Create client by passing API key directly
        var client = new HistoricalClientBuilder()
            .WithApiKey(apiKey)  // Passing key directly
            .Build();

        Console.WriteLine("✅ Successfully created Historical client with API key argument");
        Console.WriteLine("   ⚠️  Remember: Never hardcode API keys in production code!");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("⚠️  Skipping - DATABENTO_API_KEY not set");
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 3: Advanced Configuration
// ============================================================================

Console.WriteLine("Example 3: Advanced Client Configuration");
Console.WriteLine("-----------------------------------------");
Console.WriteLine("Demonstrates additional configuration options available.");
Console.WriteLine();

try
{
    var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY");

    if (!string.IsNullOrEmpty(apiKey))
    {
        // Create client with advanced configuration
        var client = new HistoricalClientBuilder()
            .WithApiKey(apiKey)
            .WithGateway(HistoricalGateway.Bo1)  // Default gateway
            .WithTimeout(TimeSpan.FromSeconds(60))  // Custom timeout
            .WithUpgradePolicy(VersionUpgradePolicy.Upgrade)  // Version policy
            .Build();

        Console.WriteLine("✅ Created client with custom configuration:");
        Console.WriteLine("   - Gateway: Bo1 (Boston datacenter)");
        Console.WriteLine("   - Timeout: 60 seconds");
        Console.WriteLine("   - Upgrade Policy: Upgrade");
        Console.WriteLine();

        // Test the client
        var datasets = await client.ListDatasetsAsync();
        Console.WriteLine($"   ✅ Client is working! Found {datasets.Count()} datasets");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("⚠️  Skipping - DATABENTO_API_KEY not set");
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Configuration Options Summary
// ============================================================================

Console.WriteLine("=== Configuration Options ===");
Console.WriteLine();
Console.WriteLine("HistoricalClientBuilder provides the following configuration methods:");
Console.WriteLine();
Console.WriteLine("1. WithApiKey(string key)");
Console.WriteLine("   - Sets the API key for authentication");
Console.WriteLine("   - Can read from environment variable: Environment.GetEnvironmentVariable(\"DATABENTO_API_KEY\")");
Console.WriteLine("   - Required parameter");
Console.WriteLine();
Console.WriteLine("2. WithGateway(HistoricalGateway gateway)");
Console.WriteLine("   - Sets the historical gateway to connect to");
Console.WriteLine("   - Currently only Bo1 (Boston) is supported");
Console.WriteLine("   - Default: HistoricalGateway.Bo1");
Console.WriteLine();
Console.WriteLine("3. WithTimeout(TimeSpan timeout)");
Console.WriteLine("   - Sets the HTTP request timeout");
Console.WriteLine("   - Default: 30 seconds");
Console.WriteLine("   - Recommended: 60+ seconds for large queries");
Console.WriteLine();
Console.WriteLine("4. WithUpgradePolicy(VersionUpgradePolicy policy)");
Console.WriteLine("   - Controls automatic DBN schema version upgrades");
Console.WriteLine("   - Options: Upgrade (default) or AsIs");
Console.WriteLine("   - Upgrade: Automatically upgrades to latest schema version");
Console.WriteLine();
Console.WriteLine("5. WithUserAgent(string userAgent)");
Console.WriteLine("   - Adds custom user agent string to HTTP requests");
Console.WriteLine("   - Optional - for tracking/debugging purposes");
Console.WriteLine();

// ============================================================================
// Best Practices
// ============================================================================

Console.WriteLine("=== Best Practices ===");
Console.WriteLine();
Console.WriteLine("✅ DO:");
Console.WriteLine("   - Store API keys in environment variables");
Console.WriteLine("   - Use HistoricalClientBuilder for all client creation");
Console.WriteLine("   - Set appropriate timeouts for your use case");
Console.WriteLine("   - Reuse client instances when possible (they are thread-safe)");
Console.WriteLine("   - Dispose clients properly when done");
Console.WriteLine();
Console.WriteLine("❌ DON'T:");
Console.WriteLine("   - Hardcode API keys in source code");
Console.WriteLine("   - Commit API keys to version control");
Console.WriteLine("   - Create new clients for every request");
Console.WriteLine("   - Share API keys across different applications");
Console.WriteLine();

Console.WriteLine("=== Historical Client Example Complete ===");
