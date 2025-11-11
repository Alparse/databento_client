using Databento.Client.Models;

namespace Databento.Client.Events;

/// <summary>
/// Event args for data received events
/// </summary>
public class DataReceivedEventArgs : EventArgs
{
    /// <summary>
    /// The received record
    /// </summary>
    public Record Record { get; }

    public DataReceivedEventArgs(Record record)
    {
        Record = record ?? throw new ArgumentNullException(nameof(record));
    }
}
