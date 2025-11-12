using System.Runtime.InteropServices;

namespace Databento.Interop.Native;

/// <summary>
/// Callback invoked when a record is received from the native library
/// </summary>
/// <param name="recordBytes">Pointer to raw record data</param>
/// <param name="recordLength">Length of record data in bytes</param>
/// <param name="recordType">Record type identifier</param>
/// <param name="userData">User-provided context pointer</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void RecordCallbackDelegate(
    byte* recordBytes,
    nuint recordLength,
    byte recordType,
    IntPtr userData);

/// <summary>
/// Callback invoked when an error occurs in the native library
/// </summary>
/// <param name="errorMessage">Error message string</param>
/// <param name="errorCode">Error code</param>
/// <param name="userData">User-provided context pointer</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ErrorCallbackDelegate(
    [MarshalAs(UnmanagedType.LPUTF8Str)] string errorMessage,
    int errorCode,
    IntPtr userData);

/// <summary>
/// Callback invoked when session metadata is received from live client (Phase 15)
/// </summary>
/// <param name="metadataJson">JSON string containing session metadata</param>
/// <param name="metadataLength">Length of metadata string in bytes</param>
/// <param name="userData">User-provided context pointer</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MetadataCallbackDelegate(
    [MarshalAs(UnmanagedType.LPUTF8Str)] string metadataJson,
    nuint metadataLength,
    IntPtr userData);
