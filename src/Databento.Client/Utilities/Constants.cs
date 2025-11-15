namespace Databento.Client.Utilities;

/// <summary>
/// Common constants used throughout the Databento client
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Standard size for error buffers when calling native methods.
    /// Set to 2048 bytes to accommodate full error context and stack traces.
    /// </summary>
    public const int ErrorBufferSize = 2048;

    /// <summary>
    /// Maximum reasonable size for a single record (10 MB).
    /// Records larger than this are rejected as likely corrupted.
    /// </summary>
    public const int MaxReasonableRecordSize = 10 * 1024 * 1024;

    /// <summary>
    /// Default buffer size for reading records from DBN files (8 KB).
    /// </summary>
    public const int RecordBufferSize = 8192;
}
