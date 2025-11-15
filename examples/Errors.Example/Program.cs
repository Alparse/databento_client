using Databento.Client.Builders;
using Databento.Interop;

Console.WriteLine("=== Databento Error Handling Example ===");
Console.WriteLine();

// Example 1: Invalid API key (401 Unauthorized)
Console.WriteLine("Example 1: Invalid API Key");
Console.WriteLine("---------------------------");

try
{
    // Create client with invalid API key
    var client = new HistoricalClientBuilder()
        .WithApiKey("invalid")
        .Build();

    Console.WriteLine("Attempting to list datasets with invalid API key...");

    // This will throw an AuthenticationException (401)
    var datasets = await client.ListDatasetsAsync();

    // This line won't be reached
    Console.WriteLine($"Success: Found {datasets.Count} datasets");
}
catch (AuthenticationException ex)
{
    // Specific exception for authentication failures
    Console.WriteLine($"Authentication Error (HTTP {ex.ErrorCode}):");
    Console.WriteLine($"  {ex.Message}");
    Console.WriteLine();
}
catch (DbentoException ex)
{
    // General Databento exception
    Console.WriteLine($"Databento Error:");
    if (ex.ErrorCode.HasValue)
    {
        Console.WriteLine($"  Error Code: {ex.ErrorCode.Value}");
    }
    Console.WriteLine($"  Message: {ex.Message}");
    Console.WriteLine();
}
catch (Exception ex)
{
    // Unexpected exception
    Console.WriteLine($"Unexpected Error: {ex.Message}");
    Console.WriteLine();
}

// Example 2: Valid authentication with proper error handling
Console.WriteLine("Example 2: Proper Error Handling");
Console.WriteLine("---------------------------------");

try
{
    var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY");

    if (string.IsNullOrEmpty(apiKey))
    {
        Console.WriteLine("DATABENTO_API_KEY environment variable not set.");
        Console.WriteLine("Skipping valid authentication example.");
        return;
    }

    var client = new HistoricalClientBuilder()
        .WithApiKey(apiKey)
        .Build();

    Console.WriteLine("Successfully authenticated with valid API key");

    var datasets = await client.ListDatasetsAsync();
    Console.WriteLine($"Retrieved {datasets.Count} datasets successfully");
}
catch (AuthenticationException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
}
catch (RateLimitException ex)
{
    Console.WriteLine($"Rate limit exceeded (HTTP {ex.ErrorCode}): {ex.Message}");
}
catch (NotFoundException ex)
{
    Console.WriteLine($"Resource not found (HTTP {ex.ErrorCode}): {ex.Message}");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation error (HTTP {ex.ErrorCode}): {ex.Message}");
}
catch (ServerException ex)
{
    Console.WriteLine($"Server error (HTTP {ex.ErrorCode}): {ex.Message}");
}
catch (DbentoException ex)
{
    Console.WriteLine($"Databento error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== Error Handling Examples Complete ===");
