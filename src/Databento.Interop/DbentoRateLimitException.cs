namespace Databento.Interop;

/// <summary>
/// Exception thrown when API rate limits are exceeded
/// </summary>
public sealed class DbentoRateLimitException : DbentoException
{
    /// <summary>
    /// When the rate limit will reset (if known)
    /// </summary>
    public DateTimeOffset? RetryAfter { get; }

    public DbentoRateLimitException(string message) : base(message)
    {
    }

    public DbentoRateLimitException(string message, int errorCode) : base(message, errorCode)
    {
    }

    public DbentoRateLimitException(string message, DateTimeOffset? retryAfter)
        : base(message)
    {
        RetryAfter = retryAfter;
    }

    public DbentoRateLimitException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
