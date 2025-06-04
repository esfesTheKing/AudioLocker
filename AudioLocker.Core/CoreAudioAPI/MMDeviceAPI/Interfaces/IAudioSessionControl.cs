using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
public partial interface IAudioSessionControl
{
    //return: AudioSessionState
    [return: MarshalAs(UnmanagedType.Interface)]
    object GetState();
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetDisplayName();
    void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetIconPath();
    void SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
    Guid GetGroupingParam();
    void SetGroupingParam(ref Guid Override, ref Guid EventContext);
    void RegisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
    void UnregisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
}
