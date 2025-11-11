using Databento.Client.Historical;

namespace Databento.Client.Builders;

/// <summary>
/// Builder for creating HistoricalClient instances
/// </summary>
public sealed class HistoricalClientBuilder
{
    private string? _apiKey;

    /// <summary>
    /// Set the Databento API key
    /// </summary>
    public HistoricalClientBuilder WithApiKey(string apiKey)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        return this;
    }

    /// <summary>
    /// Build the HistoricalClient instance
    /// </summary>
    public IHistoricalClient Build()
    {
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("API key is required. Call WithApiKey() before Build().");

        return new HistoricalClient(_apiKey);
    }
}
