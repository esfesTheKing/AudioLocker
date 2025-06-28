using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessionmanager2
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\audiopolicy.idl"
[GeneratedComInterface]
[Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
public partial interface IAudioSessionManager2 : IAudioSessionManager
{
    IAudioSessionEnumerator GetSessionEnumerator();

    void RegisterSessionNotification(IAudioSessionNotification SessionNotification);

    void UnregisterSessionNotification(IAudioSessionNotification SessionNotification);
}
