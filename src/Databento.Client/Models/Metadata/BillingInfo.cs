namespace Databento.Client.Models.Metadata;

/// <summary>
/// Billing information for a data query
/// </summary>
public class BillingInfo
{
    /// <summary>
    /// Number of records in the query result
    /// </summary>
    public ulong RecordCount { get; set; }

    /// <summary>
    /// Billable size in bytes
    /// </summary>
    public ulong BillableSizeBytes { get; set; }

    /// <summary>
    /// Estimated cost in USD
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// Billable size in megabytes
    /// </summary>
    public double BillableSizeMB => BillableSizeBytes / 1_048_576.0;

    /// <summary>
    /// Billable size in gigabytes
    /// </summary>
    public double BillableSizeGB => BillableSizeBytes / 1_073_741_824.0;

    public override string ToString()
    {
        return $"{RecordCount:N0} records, {BillableSizeMB:F2} MB, ${Cost:F4}";
    }
}
