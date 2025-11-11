using Databento.Client.Live;

namespace Databento.Client.Builders;

/// <summary>
/// Builder for creating LiveClient instances
/// </summary>
public sealed class LiveClientBuilder
{
    private string? _apiKey;

    /// <summary>
    /// Set the Databento API key
    /// </summary>
    public LiveClientBuilder WithApiKey(string apiKey)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        return this;
    }

    /// <summary>
    /// Build the LiveClient instance
    /// </summary>
    public ILiveClient Build()
    {
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("API key is required. Call WithApiKey() before Build().");

        return new LiveClient(_apiKey);
    }
}
