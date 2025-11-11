namespace Databento.Client.Models.Metadata;

/// <summary>
/// Dataset availability condition information
/// </summary>
public class DatasetConditionInfo
{
    /// <summary>
    /// Dataset code
    /// </summary>
    public string Dataset { get; set; } = string.Empty;

    /// <summary>
    /// Current condition status
    /// </summary>
    public DatasetCondition Condition { get; set; }

    /// <summary>
    /// Last modified timestamp
    /// </summary>
    public DateTimeOffset? LastModified { get; set; }

    /// <summary>
    /// Additional information or message
    /// </summary>
    public string? Message { get; set; }

    public override string ToString()
    {
        var lastMod = LastModified.HasValue ? $" (modified {LastModified.Value:yyyy-MM-dd HH:mm:ss})" : "";
        return $"{Dataset}: {Condition}{lastMod}";
    }
}
