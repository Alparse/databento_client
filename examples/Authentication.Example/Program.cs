using Databento.Client.Builders;

// Establish connection and authenticate using API key from environment variable
var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
    ?? throw new InvalidOperationException(
        "DATABENTO_API_KEY environment variable is not set. " +
        "Set it with your API key to authenticate.");

var client = new HistoricalClientBuilder()
    .WithApiKey(apiKey)
    .Build();

Console.WriteLine("Successfully authenticated with Databento API");
Console.WriteLine();

// Authenticated request - List all available datasets
Console.WriteLine("Fetching available datasets...");
var datasets = await client.ListDatasetsAsync();

Console.WriteLine($"Available datasets ({datasets.Count}):");
foreach (var dataset in datasets)
{
    Console.Write($"{dataset}, ");
}
Console.WriteLine();
