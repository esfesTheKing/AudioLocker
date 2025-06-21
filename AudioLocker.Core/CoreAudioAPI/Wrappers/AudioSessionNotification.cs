using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

[GeneratedComClass]
public partial class AudioSessionNotification(AudioSessionManager parent) : IAudioSessionNotification
{
    public void OnSessionCreated(IAudioSessionControl NewSession)
    {
        var session = new AudioSessionControl(NewSession);

        parent.OnSessionCreated?.Invoke(parent, session);
    }
}
