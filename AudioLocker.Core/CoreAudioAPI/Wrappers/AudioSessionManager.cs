using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

[GeneratedComClass]
public partial class AudioSessionNotification(AudioSessionManager parent) : IAudioSessionNotification
{
    public void OnSessionCreated(IAudioSessionControl NewSession)
    {
        parent.OnSessionCreated?.Invoke(parent, NewSession);
    }
}

public class AudioSessionManager : IDisposable
{
    private readonly IAudioSessionManager2 _sessionManager;

    public Action<object, IAudioSessionControl>? OnSessionCreated;

    private readonly AudioSessionNotification _notification;

    public AudioSessionManager(IAudioSessionManager sessionManager)
        : this((IAudioSessionManager2)sessionManager)
    { }

    public AudioSessionManager(IAudioSessionManager2 sessionManager)
    {
        _sessionManager = sessionManager;
        _notification =  new AudioSessionNotification(this);

        _sessionManager.RegisterSessionNotification(_notification);
    }

    public AudioSessionCollection Sessions
    {
        get => new AudioSessionCollection(_sessionManager.GetSessionEnumerator());
    }

    public void Dispose()
    {
        _sessionManager.UnregisterSessionNotification(_notification);
        GC.SuppressFinalize(this);
    }
}
