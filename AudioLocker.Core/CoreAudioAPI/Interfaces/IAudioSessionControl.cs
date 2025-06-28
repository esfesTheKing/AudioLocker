using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessioncontrol
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\audiopolicy.idl"
[GeneratedComInterface]
[Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
public partial interface IAudioSessionControl
{
    AudioSessionState GetState();

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetDisplayName();

    void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Value, Guid EventContext);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetIconPath();

    void SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, Guid EventContext);

    Guid GetGroupingParam();

    void SetGroupingParam(Guid Override, Guid EventContext);

    void RegisterAudioSessionNotification(IAudioSessionEvents NewNotifications);

    void UnregisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
}
