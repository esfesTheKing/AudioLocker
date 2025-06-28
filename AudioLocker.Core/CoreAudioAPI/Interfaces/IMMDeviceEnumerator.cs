using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/mmdeviceapi/nn-mmdeviceapi-immdeviceenumerator
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\mmdeviceapi.idl"
[GeneratedComInterface]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
public partial interface IMMDeviceEnumerator
{
    IMMDeviceCollection EnumAudioEndpoints(EDataFlow dataFlow, DeviceState dwStateMask);

    IMMDevice GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role);

    IMMDevice GetDevice([MarshalAs(UnmanagedType.LPWStr)] string pwstrId);

    int RegisterEndpointNotificationCallback(IMMNotificationClient pClient);

    int UnregisterEndpointNotificationCallback(IMMNotificationClient pClient);
}
