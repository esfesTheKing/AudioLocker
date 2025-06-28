using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessionevents
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\audiopolicy.idl"
[GeneratedComInterface]
[Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
public partial interface IAudioSessionEvents
{
    void OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, Guid EventContext);

    void OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, Guid EventContext);

    void OnSimpleVolumeChanged(float NewVolume, [MarshalAs(UnmanagedType.Bool)] bool NewMute, Guid EventContext);

    void OnChannelVolumeChanged(uint ChannelCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] float[] NewChannelVolumeArray, uint ChangedChannel, Guid EventContext);

    void OnGroupingParamChanged(Guid NewGroupingParam, Guid EventContext);

    void OnStateChanged(AudioSessionState NewState);

    void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);
}
