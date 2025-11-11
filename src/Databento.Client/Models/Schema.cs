namespace Databento.Client.Models;

/// <summary>
/// Databento schema types
/// </summary>
public enum Schema
{
    /// <summary>Market by order</summary>
    Mbo,

    /// <summary>Market by price level 1</summary>
    Mbp1,

    /// <summary>Market by price level 10</summary>
    Mbp10,

    /// <summary>Trades</summary>
    Trades,

    /// <summary>OHLCV 1 second bars</summary>
    Ohlcv1S,

    /// <summary>OHLCV 1 minute bars</summary>
    Ohlcv1M,

    /// <summary>OHLCV 1 hour bars</summary>
    Ohlcv1H,

    /// <summary>OHLCV 1 day bars</summary>
    Ohlcv1D,

    /// <summary>OHLCV end of day bars</summary>
    OhlcvEod,

    /// <summary>Definition schema</summary>
    Definition,

    /// <summary>Statistics schema</summary>
    Statistics,

    /// <summary>Status schema</summary>
    Status,

    /// <summary>Imbalance schema</summary>
    Imbalance
}

/// <summary>
/// Extension methods for Schema enum
/// </summary>
public static class SchemaExtensions
{
    /// <summary>
    /// Convert schema enum to string representation
    /// </summary>
    public static string ToSchemaString(this Schema schema)
    {
        return schema switch
        {
            Schema.Mbo => "mbo",
            Schema.Mbp1 => "mbp-1",
            Schema.Mbp10 => "mbp-10",
            Schema.Trades => "trades",
            Schema.Ohlcv1S => "ohlcv-1s",
            Schema.Ohlcv1M => "ohlcv-1m",
            Schema.Ohlcv1H => "ohlcv-1h",
            Schema.Ohlcv1D => "ohlcv-1d",
            Schema.OhlcvEod => "ohlcv-eod",
            Schema.Definition => "definition",
            Schema.Statistics => "statistics",
            Schema.Status => "status",
            Schema.Imbalance => "imbalance",
            _ => throw new ArgumentOutOfRangeException(nameof(schema))
        };
    }

    /// <summary>
    /// Parse schema from string
    /// </summary>
    public static Schema ParseSchema(string schemaString)
    {
        return schemaString.ToLowerInvariant() switch
        {
            "mbo" => Schema.Mbo,
            "mbp-1" => Schema.Mbp1,
            "mbp-10" => Schema.Mbp10,
            "trades" => Schema.Trades,
            "ohlcv-1s" => Schema.Ohlcv1S,
            "ohlcv-1m" => Schema.Ohlcv1M,
            "ohlcv-1h" => Schema.Ohlcv1H,
            "ohlcv-1d" => Schema.Ohlcv1D,
            "ohlcv-eod" => Schema.OhlcvEod,
            "definition" => Schema.Definition,
            "statistics" => Schema.Statistics,
            "status" => Schema.Status,
            "imbalance" => Schema.Imbalance,
            _ => throw new ArgumentException($"Unknown schema: {schemaString}", nameof(schemaString))
        };
    }
}
