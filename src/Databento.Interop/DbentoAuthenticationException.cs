namespace Databento.Interop;

/// <summary>
/// Exception thrown when authentication fails (invalid or deactivated API key)
/// </summary>
public sealed class DbentoAuthenticationException : DbentoException
{
    public DbentoAuthenticationException(string message) : base(message)
    {
    }

    public DbentoAuthenticationException(string message, int errorCode) : base(message, errorCode)
    {
    }

    public DbentoAuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
