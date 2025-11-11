namespace Databento.Client.Models.Metadata;

/// <summary>
/// Field metadata information
/// </summary>
public class FieldDetail
{
    /// <summary>
    /// Field name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Field type name (e.g., "int64", "string", "fixed_price")
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// Field encoding type
    /// </summary>
    public string EncodingType { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Name} ({TypeName})";
    }
}
