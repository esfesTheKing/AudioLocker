using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
public partial interface IAudioSessionEvents
{
    void OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, ref Guid EventContext);
    void OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, ref Guid EventContext);
    void OnSimpleVolumeChanged(float NewVolume, [MarshalAs(UnmanagedType.Bool)] bool NewMute, ref Guid EventContext);
    // NewChannelVolumeArray = float NewChannelVolumeArray[*]
    void OnChannelVolumeChanged(uint ChannelCount, IntPtr NewChannelVolumeArray, uint ChangedChannel, ref Guid EventContext);
    void OnGroupingParamChanged(ref Guid NewGroupingParam, ref Guid EventContext);
    void OnStateChanged(AudioSessionState NewState);
    void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);

}
