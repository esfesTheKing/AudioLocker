using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
public partial interface IMMDevice
{
    int Activate(ref Guid interfaceId, ClsCtx clsCtx, IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
    int GetId([MarshalAs(UnmanagedType.LPWStr)] out string id);
    int GetState(out DeviceState state);

}
