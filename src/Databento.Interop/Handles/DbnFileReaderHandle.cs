using System.Runtime.InteropServices;
using Databento.Interop.Native;

namespace Databento.Interop.Handles;

/// <summary>
/// SafeHandle wrapper for native DBN file reader
/// </summary>
public sealed class DbnFileReaderHandle : SafeHandle
{
    public DbnFileReaderHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public DbnFileReaderHandle(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            NativeMethods.dbento_dbn_file_close(handle);
        }
        return true;
    }
}
