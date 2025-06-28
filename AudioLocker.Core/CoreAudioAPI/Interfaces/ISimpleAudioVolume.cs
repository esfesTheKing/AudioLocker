using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/audioclient/nn-audioclient-isimpleaudiovolume
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\Audioclient.idl"
[GeneratedComInterface]
[Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
public partial interface ISimpleAudioVolume
{
    void SetMasterVolume(float fLevel, Guid EventContext);

    float GetMasterVolume();

    void SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, Guid EventContext);

    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetMute();
}
