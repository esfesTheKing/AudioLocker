using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

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
