using System.Runtime.InteropServices;
using Databento.Interop.Native;

namespace Databento.Interop.Handles;

/// <summary>
/// SafeHandle wrapper for native UnitPrices handle
/// </summary>
public sealed class UnitPricesHandle : SafeHandle
{
    public UnitPricesHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public UnitPricesHandle(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            NativeMethods.dbento_unit_prices_destroy(handle);
        }
        return true;
    }
}
