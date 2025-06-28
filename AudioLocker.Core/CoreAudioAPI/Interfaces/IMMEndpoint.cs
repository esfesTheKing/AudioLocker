using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/mmdeviceapi/nn-mmdeviceapi-immendpoint
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\mmdeviceapi.idl"
[GeneratedComInterface]
[Guid("1BE09788-6894-4089-8586-9A2A6C265AC5")]
public partial interface IMMEndpoint
{
    EDataFlow GetDataFlow();
}
