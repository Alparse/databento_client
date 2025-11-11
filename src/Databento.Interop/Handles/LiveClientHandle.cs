using System.Runtime.InteropServices;
using Databento.Interop.Native;

namespace Databento.Interop.Handles;

/// <summary>
/// SafeHandle wrapper for native live client
/// </summary>
public sealed class LiveClientHandle : SafeHandle
{
    public LiveClientHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public LiveClientHandle(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            NativeMethods.dbento_live_destroy(handle);
        }
        return true;
    }
}
