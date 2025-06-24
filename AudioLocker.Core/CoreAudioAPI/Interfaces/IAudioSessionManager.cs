using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessionmanager
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\audiopolicy.idl"
[GeneratedComInterface]
[Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4")]
public partial interface IAudioSessionManager
{
    IAudioSessionControl GetAudioSessionControl(Guid AudioSessionGuid, StreamFlags StreamFlags);

    ISimpleAudioVolume GetSimpleAudioVolume(Guid AudioSessionGuid, StreamFlags StreamFlags);
}
