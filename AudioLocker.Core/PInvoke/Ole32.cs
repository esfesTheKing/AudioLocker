using System.Runtime.InteropServices;

namespace AudioLocker.Core.PInvoke;

internal static partial class Ole32
{
    // https://learn.microsoft.com/en-us/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
    // C Definition: 'C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\combaseapi.h"
    [LibraryImport(nameof(Ole32))]
    public static partial int CoCreateInstance(ref Guid rclsid, IntPtr pUnkOuter, int dwClsContext, ref Guid riid, out IntPtr ppObj);
}
