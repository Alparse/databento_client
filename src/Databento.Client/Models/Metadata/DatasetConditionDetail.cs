namespace Databento.Client.Models.Metadata;

/// <summary>
/// Dataset condition information for a specific date
/// </summary>
public class DatasetConditionDetail
{
    /// <summary>
    /// The date of the described data (ISO 8601 date string)
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// The condition code describing the quality and availability of data on this date
    /// </summary>
    public DatasetCondition Condition { get; set; }

    /// <summary>
    /// The date when any schema in the dataset on the given day was last modified (optional)
    /// </summary>
    public string? LastModifiedDate { get; set; }

    public override string ToString()
    {
        var lastMod = !string.IsNullOrEmpty(LastModifiedDate) ? $" (modified {LastModifiedDate})" : "";
        return $"{Date}: {Condition}{lastMod}";
    }
}
