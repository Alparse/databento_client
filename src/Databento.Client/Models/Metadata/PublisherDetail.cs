namespace Databento.Client.Models.Metadata;

/// <summary>
/// Publisher information
/// </summary>
public class PublisherDetail
{
    /// <summary>
    /// Publisher identifier
    /// </summary>
    public ushort PublisherId { get; set; }

    /// <summary>
    /// Dataset code
    /// </summary>
    public string Dataset { get; set; } = string.Empty;

    /// <summary>
    /// Venue code
    /// </summary>
    public string Venue { get; set; } = string.Empty;

    /// <summary>
    /// Publisher description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Publisher {PublisherId}: {Dataset} ({Venue}) - {Description}";
    }
}
