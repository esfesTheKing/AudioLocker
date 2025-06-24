using System.Runtime.InteropServices;

namespace AudioLocker.Core.PInvoke;

internal static partial class Ole32
{
    [LibraryImport(nameof(Ole32))]
    public static partial int CoCreateInstance(ref Guid rclsid, IntPtr pUnkOuter, int dwClsContext, ref Guid riid, out IntPtr ppObj);
}
