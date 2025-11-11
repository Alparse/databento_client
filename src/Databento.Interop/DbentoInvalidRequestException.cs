namespace Databento.Interop;

/// <summary>
/// Exception thrown when a request is invalid (bad parameters, unsupported schema, etc.)
/// </summary>
public sealed class DbentoInvalidRequestException : DbentoException
{
    public DbentoInvalidRequestException(string message) : base(message)
    {
    }

    public DbentoInvalidRequestException(string message, int errorCode) : base(message, errorCode)
    {
    }

    public DbentoInvalidRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
