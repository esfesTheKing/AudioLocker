using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Wrappers.Interfaces;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionControl : IDisposable
{
    public SimpleAudioVolume SimpleAudioVolume;

    public readonly string SessionInstanceIdentifier;
    public readonly uint ProcessId;

    private readonly IAudioSessionControl2 _audioSession;
    private AudioSessionEventsCallback? _eventsCallback;

    internal Action<object>? OnSessionDisconnect;

    public AudioSessionControl(IAudioSessionControl audioSession)
        : this((IAudioSessionControl2)audioSession)
    { }

    public AudioSessionControl(IAudioSessionControl2 audioSession)
    {
        _audioSession = audioSession;

        SessionInstanceIdentifier = _audioSession.GetSessionInstanceIdentifier();
        ProcessId = _audioSession.GetProcessId();

        SimpleAudioVolume = new SimpleAudioVolume((ISimpleAudioVolume)audioSession);
    }

    public override int GetHashCode() => SessionInstanceIdentifier.GetHashCode();

    public void RegisterEventClient(IAudioSessionEventsHandler eventClient)
    {
        UnRegisterEventClient();

        _eventsCallback = new AudioSessionEventsCallback(eventClient);
        _eventsCallback.OnSessionDisconnect += OnSessionDisconnectCallback;

        _audioSession.RegisterAudioSessionNotification(_eventsCallback);
    }

    public void UnRegisterEventClient()
    {
        if (_eventsCallback is not null)
        {
            _audioSession.UnregisterAudioSessionNotification(_eventsCallback);

            _eventsCallback.OnSessionDisconnect -= OnSessionDisconnectCallback;
            _eventsCallback = null;
        }
    }

    public void Dispose()
    {
        UnRegisterEventClient();
        OnSessionDisconnect?.Invoke(this);

        GC.SuppressFinalize(this);
    }

    private void OnSessionDisconnectCallback()
    {
        OnSessionDisconnect?.Invoke(this);
    }
}
