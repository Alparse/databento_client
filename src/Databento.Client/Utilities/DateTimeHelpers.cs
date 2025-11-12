namespace Databento.Client.Utilities;

/// <summary>
/// Helper utilities for datetime conversion and manipulation
/// </summary>
public static class DateTimeHelpers
{
    /// <summary>
    /// Convert Unix nanoseconds to DateTimeOffset
    /// </summary>
    /// <param name="unixNanos">Unix timestamp in nanoseconds</param>
    /// <returns>DateTimeOffset representing the timestamp</returns>
    public static DateTimeOffset FromUnixNanos(long unixNanos)
    {
        long milliseconds = unixNanos / 1_000_000;
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }

    /// <summary>
    /// Convert DateTimeOffset to Unix nanoseconds
    /// </summary>
    /// <param name="dateTime">DateTimeOffset to convert</param>
    /// <returns>Unix timestamp in nanoseconds</returns>
    /// <exception cref="OverflowException">Thrown when the timestamp cannot be represented as nanoseconds</exception>
    public static long ToUnixNanos(DateTimeOffset dateTime)
    {
        // HIGH FIX: Use checked arithmetic to prevent overflow for extreme dates
        return checked(dateTime.ToUnixTimeMilliseconds() * 1_000_000);
    }

    /// <summary>
    /// Convert Unix nanoseconds to DateTime (UTC)
    /// </summary>
    /// <param name="unixNanos">Unix timestamp in nanoseconds</param>
    /// <returns>DateTime representing the timestamp in UTC</returns>
    public static DateTime FromUnixNanosUtc(long unixNanos)
    {
        long milliseconds = unixNanos / 1_000_000;
        return DateTime.UnixEpoch.AddMilliseconds(milliseconds);
    }

    /// <summary>
    /// Convert DateTime to Unix nanoseconds
    /// </summary>
    /// <param name="dateTime">DateTime to convert (will be treated as UTC if unspecified)</param>
    /// <returns>Unix timestamp in nanoseconds</returns>
    /// <exception cref="OverflowException">Thrown when the timestamp cannot be represented as nanoseconds</exception>
    public static long ToUnixNanos(DateTime dateTime)
    {
        var utcDateTime = dateTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            : dateTime.ToUniversalTime();

        // HIGH FIX: Use checked arithmetic to prevent overflow for extreme dates
        return checked((long)(utcDateTime - DateTime.UnixEpoch).TotalMilliseconds * 1_000_000);
    }

    /// <summary>
    /// Convert DateOnly to Unix nanoseconds (midnight UTC)
    /// </summary>
    /// <param name="date">DateOnly to convert</param>
    /// <returns>Unix timestamp in nanoseconds (midnight UTC)</returns>
    public static long ToUnixNanos(DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return ToUnixNanos(dateTime);
    }

    /// <summary>
    /// Convert Unix nanoseconds to DateOnly (UTC)
    /// </summary>
    /// <param name="unixNanos">Unix timestamp in nanoseconds</param>
    /// <returns>DateOnly representing the date in UTC</returns>
    public static DateOnly FromUnixNanosToDateOnly(long unixNanos)
    {
        var dateTime = FromUnixNanosUtc(unixNanos);
        return DateOnly.FromDateTime(dateTime);
    }

    /// <summary>
    /// Get the start of the day for a given date (midnight UTC)
    /// </summary>
    /// <param name="date">The date</param>
    /// <returns>DateTimeOffset at midnight UTC for that date</returns>
    public static DateTimeOffset StartOfDay(DateOnly date)
    {
        return new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
    }

    /// <summary>
    /// Get the end of the day for a given date (one nanosecond before midnight UTC)
    /// </summary>
    /// <param name="date">The date</param>
    /// <returns>DateTimeOffset at the last moment of that date</returns>
    public static DateTimeOffset EndOfDay(DateOnly date)
    {
        return new DateTimeOffset(date.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
    }

    /// <summary>
    /// Convert a date range to Unix nanosecond timestamps
    /// </summary>
    /// <param name="startDate">Start date (inclusive)</param>
    /// <param name="endDate">End date (exclusive)</param>
    /// <returns>Tuple of (start nanos, end nanos)</returns>
    public static (long StartNanos, long EndNanos) DateRangeToUnixNanos(DateOnly startDate, DateOnly endDate)
    {
        return (ToUnixNanos(startDate), ToUnixNanos(endDate));
    }

    /// <summary>
    /// Convert a datetime range to Unix nanosecond timestamps
    /// </summary>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    /// <returns>Tuple of (start nanos, end nanos)</returns>
    public static (long StartNanos, long EndNanos) DateTimeRangeToUnixNanos(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        return (ToUnixNanos(startTime), ToUnixNanos(endTime));
    }
}
