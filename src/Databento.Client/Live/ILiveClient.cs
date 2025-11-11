using Databento.Client.Events;
using Databento.Client.Models;

namespace Databento.Client.Live;

/// <summary>
/// Live streaming client for real-time market data
/// </summary>
public interface ILiveClient : IAsyncDisposable
{
    /// <summary>
    /// Event fired when data is received
    /// </summary>
    event EventHandler<DataReceivedEventArgs>? DataReceived;

    /// <summary>
    /// Event fired when an error occurs
    /// </summary>
    event EventHandler<Events.ErrorEventArgs>? ErrorOccurred;

    /// <summary>
    /// Subscribe to a data stream
    /// </summary>
    /// <param name="dataset">Dataset name (e.g., "GLBX.MDP3")</param>
    /// <param name="schema">Schema type</param>
    /// <param name="symbols">List of symbols to subscribe to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SubscribeAsync(
        string dataset,
        Schema schema,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Start receiving data (non-blocking)
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop receiving data
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream records as an async enumerable
    /// </summary>
    IAsyncEnumerable<Record> StreamAsync(CancellationToken cancellationToken = default);
}
