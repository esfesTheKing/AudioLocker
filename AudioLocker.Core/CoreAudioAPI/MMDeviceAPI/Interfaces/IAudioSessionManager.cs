using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4")]
public partial interface IAudioSessionManager
{
    IAudioSessionControl GetAudioSessionControl(ref Guid AudioSessionGuid, StreamFlags StreamFlags);
    ISimpleAudioVolume GetSimpleAudioVolume(ref Guid AudioSessionGuid, StreamFlags StreamFlags);
}
