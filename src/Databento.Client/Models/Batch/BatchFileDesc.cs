using System.Text.Json.Serialization;

namespace Databento.Client.Models.Batch;

/// <summary>
/// Description of a batch file available for download
/// </summary>
public sealed class BatchFileDesc
{
    /// <summary>
    /// Filename of the batch output file
    /// </summary>
    [JsonPropertyName("filename")]
    public required string Filename { get; init; }

    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    [JsonPropertyName("size")]
    public required ulong Size { get; init; }

    /// <summary>
    /// File hash for integrity verification
    /// </summary>
    [JsonPropertyName("hash")]
    public required string Hash { get; init; }

    /// <summary>
    /// HTTPS download URL
    /// </summary>
    [JsonPropertyName("https_url")]
    public required string HttpsUrl { get; init; }

    /// <summary>
    /// FTP download URL
    /// </summary>
    [JsonPropertyName("ftp_url")]
    public required string FtpUrl { get; init; }
}
