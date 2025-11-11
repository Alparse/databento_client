namespace Databento.Client.Models;

/// <summary>
/// Base class for all Databento records
/// </summary>
public abstract class Record
{
    /// <summary>
    /// Timestamp in nanoseconds since Unix epoch
    /// </summary>
    public long TimestampNs { get; set; }

    /// <summary>
    /// Record type identifier
    /// </summary>
    public byte RType { get; set; }

    /// <summary>
    /// Publisher ID
    /// </summary>
    public ushort PublisherId { get; set; }

    /// <summary>
    /// Instrument ID
    /// </summary>
    public uint InstrumentId { get; set; }

    /// <summary>
    /// Get timestamp as DateTimeOffset
    /// </summary>
    public DateTimeOffset Timestamp =>
        DateTimeOffset.FromUnixTimeMilliseconds(TimestampNs / 1_000_000);

    /// <summary>
    /// Deserialize a record from raw bytes with the given RType
    /// </summary>
    internal static unsafe Record FromBytes(ReadOnlySpan<byte> bytes, byte rtype)
    {
        if (bytes.Length < 16)
            throw new ArgumentException($"Invalid record data - too small: {bytes.Length} bytes", nameof(bytes));

        // Read RecordHeader (16 bytes)
        // offset 0: length (uint8)
        // offset 1: rtype (uint8)
        // offset 2-3: publisher_id (uint16)
        // offset 4-7: instrument_id (uint32)
        // offset 8-15: ts_event (uint64 UnixNanos)

        ushort publisherId = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(2, 2));
        uint instrumentId = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(4, 4));
        long tsEvent = System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(8, 8));

        // Dispatch to appropriate record type based on RType
        Record result = (rtype, bytes.Length) switch
        {
            // Trade messages (48 bytes) - Mbp0 / Trades schema
            (0x00, 48) => DeserializeTradeMsg(bytes, rtype, publisherId, instrumentId, tsEvent),

            // MBO messages (56 bytes)
            (0xA0, 56) => new UnknownRecord { RType = rtype, RawData = bytes.ToArray() },

            // System/metadata messages
            _ => new UnknownRecord { RType = rtype, RawData = bytes.ToArray() }
        };

        return result;
    }

    private static TradeMessage DeserializeTradeMsg(ReadOnlySpan<byte> bytes, byte rtype,
        ushort publisherId, uint instrumentId, long tsEvent)
    {
        if (bytes.Length < 48)
            throw new ArgumentException("Invalid TradeMsg data - too small", nameof(bytes));

        // TradeMsg layout (48 bytes):
        // offset 16-23: price (int64)
        // offset 24-27: size (uint32)
        // offset 28: action (uint8/char)
        // offset 29: side (uint8/char)
        // offset 30: flags (uint8)
        // offset 31: depth (uint8)
        // offset 32-39: ts_recv (uint64)
        // offset 40-43: ts_in_delta (int32)
        // offset 44-47: sequence (uint32)

        long price = System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(16, 8));
        uint size = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(24, 4));
        char action = (char)bytes[28];
        char side = (char)bytes[29];
        byte flags = bytes[30];
        byte depth = bytes[31];
        uint sequence = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(44, 4));

        return new TradeMessage
        {
            RType = rtype,
            PublisherId = publisherId,
            InstrumentId = instrumentId,
            TimestampNs = tsEvent,
            Price = price,
            Size = size,
            Action = action,
            Side = side,
            Flags = flags,
            Depth = depth,
            Sequence = sequence
        };
    }
}

/// <summary>
/// Placeholder for unknown record types
/// </summary>
public class UnknownRecord : Record
{
    public byte[]? RawData { get; set; }
}
