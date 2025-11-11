namespace Databento.Client.Events;

/// <summary>
/// Event args for error events
/// </summary>
public class ErrorEventArgs : EventArgs
{
    /// <summary>
    /// The exception that occurred
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Error code (if available)
    /// </summary>
    public int? ErrorCode { get; }

    public ErrorEventArgs(Exception exception, int? errorCode = null)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        ErrorCode = errorCode;
    }
}
