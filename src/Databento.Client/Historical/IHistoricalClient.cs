using Databento.Client.Metadata;
using Databento.Client.Models;
using Databento.Client.Models.Metadata;

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

    /// <summary>
    /// Get metadata for a historical query
    /// Note: This feature is currently not fully implemented in the native layer
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="schema">Schema type</param>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    /// <returns>Metadata object, or null if not available</returns>
    IMetadata? GetMetadata(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime);

    // ========================================================================
    // Metadata API Methods
    // ========================================================================

    /// <summary>
    /// List all publishers
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of publisher details</returns>
    Task<IReadOnlyList<PublisherDetail>> ListPublishersAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List datasets, optionally filtered by venue
    /// </summary>
    /// <param name="venue">Optional venue filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of dataset codes</returns>
    Task<IReadOnlyList<string>> ListDatasetsAsync(
        string? venue = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List schemas available for a dataset
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available schemas</returns>
    Task<IReadOnlyList<Schema>> ListSchemasAsync(
        string dataset,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List fields for a given encoding and schema
    /// </summary>
    /// <param name="encoding">Data encoding format</param>
    /// <param name="schema">Schema type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of field details</returns>
    Task<IReadOnlyList<FieldDetail>> ListFieldsAsync(
        Encoding encoding,
        Schema schema,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get dataset availability condition
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dataset condition information</returns>
    Task<DatasetConditionInfo> GetDatasetConditionAsync(
        string dataset,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get dataset time range
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dataset time range</returns>
    Task<DatasetRange> GetDatasetRangeAsync(
        string dataset,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get record count for a query
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="schema">Schema type</param>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    /// <param name="symbols">List of symbols</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Record count</returns>
    Task<ulong> GetRecordCountAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get billable size for a query
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="schema">Schema type</param>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    /// <param name="symbols">List of symbols</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Billable size in bytes</returns>
    Task<ulong> GetBillableSizeAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cost estimate for a query
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="schema">Schema type</param>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    /// <param name="symbols">List of symbols</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cost in USD</returns>
    Task<decimal> GetCostAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get combined billing information for a query
    /// </summary>
    /// <param name="dataset">Dataset name</param>
    /// <param name="schema">Schema type</param>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    /// <param name="symbols">List of symbols</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Billing information</returns>
    Task<BillingInfo> GetBillingInfoAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default);
}
