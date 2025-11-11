namespace Databento.Client.Models;

/// <summary>
/// Constants used throughout the Databento API
/// </summary>
public static class Constants
{
    /// <summary>
    /// API version
    /// </summary>
    public const int ApiVersion = 0;

    /// <summary>
    /// Expected API key length
    /// </summary>
    public const int ApiKeyLength = 32;

    /// <summary>
    /// DBN binary format version
    /// </summary>
    public const int DbnVersion = 3;

    /// <summary>
    /// Maximum symbol string length (null-terminated)
    /// </summary>
    public const int SymbolCstrLen = 71;

    /// <summary>
    /// Maximum asset string length (null-terminated)
    /// </summary>
    public const int AssetCstrLen = 11;

    /// <summary>
    /// Undefined/sentinel price value
    /// </summary>
    public const long UndefPrice = long.MaxValue;

    /// <summary>
    /// Undefined/sentinel order size value
    /// </summary>
    public const uint UndefOrderSize = uint.MaxValue;

    /// <summary>
    /// Undefined/sentinel statistic quantity value
    /// </summary>
    public const long UndefStatQuantity = long.MaxValue;

    /// <summary>
    /// Undefined/sentinel timestamp value
    /// </summary>
    public const ulong UndefTimestamp = ulong.MaxValue;

    /// <summary>
    /// Fixed-point price scale (prices are value * 10^9)
    /// </summary>
    public const long FixedPriceScale = 1_000_000_000;

    /// <summary>
    /// Record header length multiplier (headers are multiples of 4 bytes)
    /// </summary>
    public const int RecordHeaderLengthMultiplier = 4;

    /// <summary>
    /// Convert fixed-point price to decimal
    /// </summary>
    public static decimal PriceToDecimal(long fixedPointPrice)
    {
        return fixedPointPrice / (decimal)FixedPriceScale;
    }

    /// <summary>
    /// Convert decimal price to fixed-point
    /// </summary>
    public static long DecimalToPrice(decimal decimalPrice)
    {
        return (long)(decimalPrice * FixedPriceScale);
    }

    /// <summary>
    /// Check if price is undefined
    /// </summary>
    public static bool IsPriceUndefined(long price)
    {
        return price == UndefPrice;
    }

    /// <summary>
    /// Check if timestamp is undefined
    /// </summary>
    public static bool IsTimestampUndefined(ulong timestamp)
    {
        return timestamp == UndefTimestamp;
    }
}
