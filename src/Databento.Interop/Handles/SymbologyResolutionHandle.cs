using System.Runtime.InteropServices;
using Databento.Interop.Native;

namespace Databento.Interop.Handles;

/// <summary>
/// SafeHandle wrapper for native SymbologyResolution handle
/// </summary>
public sealed class SymbologyResolutionHandle : SafeHandle
{
    public SymbologyResolutionHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public SymbologyResolutionHandle(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            NativeMethods.dbento_symbology_resolution_destroy(handle);
        }
        return true;
    }
}
