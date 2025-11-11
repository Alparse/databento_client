namespace Databento.Interop;

/// <summary>
/// Exception thrown when a Databento operation fails
/// </summary>
public class DbentoException : Exception
{
    public int? ErrorCode { get; }

    public DbentoException(string message) : base(message)
    {
    }

    public DbentoException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public DbentoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
