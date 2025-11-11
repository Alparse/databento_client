using System.Runtime.InteropServices;
using Databento.Interop.Handles;

namespace Databento.Interop.Native;

/// <summary>
/// P/Invoke declarations for databento_native library
/// </summary>
public static partial class NativeMethods
{
    private const string LibName = "databento_native";

    // ========================================================================
    // Live Client API
    // ========================================================================

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr dbento_live_create(
        string apiKey,
        byte[]? errorBuffer,
        nuint errorBufferSize);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int dbento_live_subscribe(
        LiveClientHandle handle,
        string dataset,
        string schema,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPUTF8Str)]
        string[] symbols,
        nuint symbolCount,
        byte[]? errorBuffer,
        nuint errorBufferSize);

    [LibraryImport(LibName)]
    public static partial int dbento_live_start(
        LiveClientHandle handle,
        RecordCallbackDelegate onRecord,
        ErrorCallbackDelegate onError,
        IntPtr userData,
        byte[]? errorBuffer,
        nuint errorBufferSize);

    [LibraryImport(LibName)]
    public static partial void dbento_live_stop(LiveClientHandle handle);

    [LibraryImport(LibName)]
    public static partial void dbento_live_destroy(IntPtr handle);

    // ========================================================================
    // Historical Client API
    // ========================================================================

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr dbento_historical_create(
        string apiKey,
        byte[]? errorBuffer,
        nuint errorBufferSize);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int dbento_historical_get_range(
        HistoricalClientHandle handle,
        string dataset,
        string schema,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPUTF8Str)]
        string[] symbols,
        nuint symbolCount,
        long startTimeNs,
        long endTimeNs,
        RecordCallbackDelegate onRecord,
        IntPtr userData,
        byte[]? errorBuffer,
        nuint errorBufferSize);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr dbento_historical_get_metadata(
        HistoricalClientHandle handle,
        string dataset,
        string schema,
        long startTimeNs,
        long endTimeNs,
        byte[]? errorBuffer,
        nuint errorBufferSize);

    [LibraryImport(LibName)]
    public static partial void dbento_historical_destroy(IntPtr handle);

    // ========================================================================
    // Metadata API
    // ========================================================================

    [LibraryImport(LibName)]
    public static partial int dbento_metadata_get_symbol_mapping(
        MetadataHandle handle,
        uint instrumentId,
        byte[] symbolBuffer,
        nuint symbolBufferSize);

    [LibraryImport(LibName)]
    public static partial void dbento_metadata_destroy(IntPtr handle);
}
