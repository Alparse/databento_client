using System.Runtime.InteropServices;
using Databento.Interop.Native;

namespace Databento.Interop.Handles;

/// <summary>
/// SafeHandle wrapper for native historical client
/// </summary>
public sealed class HistoricalClientHandle : SafeHandle
{
    public HistoricalClientHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public HistoricalClientHandle(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            NativeMethods.dbento_historical_destroy(handle);
        }
        return true;
    }
}
