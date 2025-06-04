using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;


[GeneratedComInterface]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
public partial interface IMMDeviceEnumerator
{
    IMMDeviceCollection EnumAudioEndpoints(EDataFlow dataFlow, DeviceState stateMask);
    IMMDevice GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role);
    IMMDevice GetDevice([MarshalAs(UnmanagedType.LPWStr)] string id);
    int RegisterEndpointNotificationCallback(IMMNotificationClient client);
    int UnregisterEndpointNotificationCallback(IMMNotificationClient client);
}
