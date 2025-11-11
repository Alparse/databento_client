using Databento.Client.Models;
using Databento.Client.Models.Dbn;

namespace Databento.Client.Dbn;

/// <summary>
/// Reader for DBN (Databento Binary) format files
/// </summary>
public interface IDbnFileReader : IDisposable
{
    /// <summary>
    /// Get metadata about the DBN file
    /// </summary>
    /// <returns>DBN file metadata</returns>
    DbnMetadata GetMetadata();

    /// <summary>
    /// Read all records from the DBN file as an async stream
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of records</returns>
    IAsyncEnumerable<Record> ReadRecordsAsync(CancellationToken cancellationToken = default);
}
