using AudioLocker.Core.CoreAudioAPI.Interfaces;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionManager : IDisposable
{
    private readonly IAudioSessionManager2 _sessionManager;
    private readonly AudioSessionCollection _sessions;

    public event Action<object, AudioSessionControl>? OnSessionCreated;

    private readonly AudioSessionNotification _notification;

    public AudioSessionManager(IAudioSessionManager sessionManager)
        : this((IAudioSessionManager2)sessionManager)
    { }

    public AudioSessionManager(IAudioSessionManager2 sessionManager)
    {
        _sessionManager = sessionManager;
        _sessions = new AudioSessionCollection(_sessionManager.GetSessionEnumerator());

        _notification = new AudioSessionNotification(this);
        _notification.OnSessionCreatedEvent += OnSessionCreatedCallback;

        _sessionManager.RegisterSessionNotification(_notification);
    }

    public AudioSessionCollection Sessions
    {
        get => _sessions;
    }

    private void OnSessionCreatedCallback(object sender, AudioSessionControl newSession)
    {
        _sessions.Add(newSession);

        OnSessionCreated?.Invoke(this, newSession);
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

        _notification.OnSessionCreatedEvent -= OnSessionCreated;
        _sessionManager.UnregisterSessionNotification(_notification);

        DisposeSessions();

        GC.SuppressFinalize(this);
    }
}
