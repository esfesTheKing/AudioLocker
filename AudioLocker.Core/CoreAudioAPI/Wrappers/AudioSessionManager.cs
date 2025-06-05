using AudioLocker.Core.CoreAudioAPI.Interfaces;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionManager
{
    private readonly IAudioSessionManager2 _sessionManager;

    public AudioSessionManager(IAudioSessionManager sessionManager)
    {
        _sessionManager = (IAudioSessionManager2)sessionManager;
    }

    public AudioSessionManager(IAudioSessionManager2 sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public AudioSessionCollection Sessions
    {
        get => new AudioSessionCollection(_sessionManager.GetSessionEnumerator());
    }
}
