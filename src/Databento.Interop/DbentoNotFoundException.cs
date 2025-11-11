namespace Databento.Interop;

/// <summary>
/// Exception thrown when a requested resource (dataset, symbol, etc.) is not found
/// </summary>
public sealed class DbentoNotFoundException : DbentoException
{
    public DbentoNotFoundException(string message) : base(message)
    {
    }

    public DbentoNotFoundException(string message, int errorCode) : base(message, errorCode)
    {
    }

    public DbentoNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
