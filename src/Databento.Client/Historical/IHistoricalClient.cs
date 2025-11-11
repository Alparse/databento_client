using Databento.Client.Models;

namespace Databento.Client.Historical;

/// <summary>
/// Historical data client for querying past market data
/// </summary>
public interface IHistoricalClient : IAsyncDisposable
{
    /// <summary>
    /// Query historical data for a time range
    /// </summary>
    /// <param name="dataset">Dataset name (e.g., "GLBX.MDP3")</param>
    /// <param name="schema">Schema type</param>
    /// <param name="symbols">List of symbols</param>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of records</returns>
    IAsyncEnumerable<Record> GetRangeAsync(
        string dataset,
        Schema schema,
        IEnumerable<string> symbols,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken cancellationToken = default);
}
