namespace Databento.Client.Models.Metadata;

/// <summary>
/// Time range for dataset availability
/// </summary>
public class DatasetRange
{
    /// <summary>
    /// Start time of available data
    /// </summary>
    public DateTimeOffset Start { get; set; }

    /// <summary>
    /// End time of available data
    /// </summary>
    public DateTimeOffset End { get; set; }

    /// <summary>
    /// Duration of available data
    /// </summary>
    public TimeSpan Duration => End - Start;

    public override string ToString()
    {
        return $"{Start:yyyy-MM-dd HH:mm:ss} to {End:yyyy-MM-dd HH:mm:ss} ({Duration.TotalDays:F1} days)";
    }
}
