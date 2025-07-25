﻿using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Wrappers.Interfaces;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionControl : IDisposable
{
    public SimpleAudioVolume SimpleAudioVolume;

    public readonly string SessionInstanceIdentifier;
    public readonly uint ProcessId;

    private readonly IAudioSessionControl2 _audioSession;
    private AudioSessionEventsCallback? _eventsCallback;

    internal event Action<object>? OnSessionDisconnect;

    public AudioSessionControl(IAudioSessionControl audioSession)
        : this((IAudioSessionControl2)audioSession)
    { }

    public AudioSessionControl(IAudioSessionControl2 audioSession)
    {
        _audioSession = audioSession;

        SessionInstanceIdentifier = _audioSession.GetSessionInstanceIdentifier();
        ProcessId = _audioSession.GetProcessId();

        // NOTES:
        //   1. "... you can query ISimpleAudioVolume interface on IAudioSessionControl interface..."
        //   2. "Using QueryInterface on a COM object is the same as performing a cast operation in managed code."
        // Sources:
        //   https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.queryinterface
        //   https://stackoverflow.com/a/65444615
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
        OnSessionDisconnectCallback();

        OnSessionDisconnect = null;

        GC.SuppressFinalize(this);
    }

    private void OnSessionDisconnectCallback()
    {
        OnSessionDisconnect?.Invoke(this);
    }
}
