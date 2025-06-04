using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Structs;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
public partial interface IMMDevice
{
    [return: MarshalAs(UnmanagedType.Interface)]
    object Activate(ref Guid iid, CLSCTX clsCtx, IntPtr activationParams);
    IPropertyStore OpenPropertyStore(STGM stgmAccess);
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetId();
    DeviceState GetState();
}

public static class IMMDeviceExtension
{
    public static T Activate<T>(this IMMDevice device)
    {
        var iid = typeof(T).GUID;
        var obj = device.Activate(ref iid, CLSCTX.ALL, IntPtr.Zero);

        return (T)obj;
    }
}
