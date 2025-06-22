using AudioLocker.Core.CoreAudioAPI.Interfaces;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionManager : IDisposable
{
    private readonly IAudioSessionManager2 _sessionManager;
    private readonly AudioSessionCollection _sessions;

    public Action<object, AudioSessionControl>? OnSessionCreated;

    private readonly AudioSessionNotification _notification;

    public AudioSessionManager(IAudioSessionManager sessionManager)
        : this((IAudioSessionManager2)sessionManager)
    { }

    public AudioSessionManager(IAudioSessionManager2 sessionManager)
    {
        _sessionManager = sessionManager;
        _notification = new AudioSessionNotification(this);

        _sessionManager.RegisterSessionNotification(_notification);
        _sessions = new AudioSessionCollection(_sessionManager.GetSessionEnumerator());

        OnSessionCreated += KeepTrackOnCreatedSessions;
    }

    public AudioSessionCollection Sessions
    {
        get => _sessions;
    }

    private void KeepTrackOnCreatedSessions(object sender, AudioSessionControl newSession)
    {
        _sessions.Add(newSession);
    }

    private void DisposeSessions()
    {
        var copy = _sessions.ToList();
        foreach (var session in copy)
        {
            session.Dispose();
        }
    }

    public void Dispose()
    {
        OnSessionCreated = null;

        _sessionManager.UnregisterSessionNotification(_notification);
        DisposeSessions();

        GC.SuppressFinalize(this);
    }
}
