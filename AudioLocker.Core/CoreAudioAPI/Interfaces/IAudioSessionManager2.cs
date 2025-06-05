using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

[GeneratedComInterface]
[Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
public partial interface IAudioSessionManager2 : IAudioSessionManager
{
    IAudioSessionEnumerator GetSessionEnumerator();
    void RegisterSessionNotification(IAudioSessionNotification SessionNotification);
    void UnregisterSessionNotification(IAudioSessionNotification SessionNotification);
}
