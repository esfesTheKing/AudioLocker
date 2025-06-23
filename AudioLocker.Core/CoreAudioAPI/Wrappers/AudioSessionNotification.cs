using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

[GeneratedComClass]
public partial class AudioSessionNotification(AudioSessionManager parent) : IAudioSessionNotification
{
    public event Action<object, AudioSessionControl>? OnSessionCreatedEvent;

    public void OnSessionCreated(IAudioSessionControl NewSession)
    {
        var session = new AudioSessionControl(NewSession);

        OnSessionCreatedEvent?.Invoke(parent, session);
    }
}
