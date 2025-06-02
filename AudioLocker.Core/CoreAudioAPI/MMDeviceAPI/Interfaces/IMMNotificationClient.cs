using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Structs;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;


[GeneratedComInterface]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
public partial interface IMMNotificationClient
{
    void OnDefaultDeviceChanged(EDataFlow dataFlow, ERole role, [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId);
    void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string deviceId);
    void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId);
    void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, DeviceState state);
    void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, PropertyKey key);
}
