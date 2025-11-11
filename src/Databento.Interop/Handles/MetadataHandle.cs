using System.Runtime.InteropServices;
using Databento.Interop.Native;

namespace Databento.Interop.Handles;

/// <summary>
/// SafeHandle wrapper for native metadata handle
/// </summary>
public sealed class MetadataHandle : SafeHandle
{
    public MetadataHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public MetadataHandle(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            NativeMethods.dbento_metadata_destroy(handle);
        }
        return true;
    }
}
