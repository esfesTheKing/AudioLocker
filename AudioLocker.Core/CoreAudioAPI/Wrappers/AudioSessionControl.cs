using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Wrappers.Interfaces;
using System.Diagnostics;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionControl : IDisposable
{
    private readonly IAudioSessionControl2 _audioSession;
    private AudioSessionEventsCallback? _eventsCallback;

    public SimpleAudioVolume SimpleAudioVolume;

    public uint ProcessId
    {
        get => _audioSession.GetProcessId();
    }

    public AudioSessionControl(IAudioSessionControl audioSession)
        : this ((IAudioSessionControl2)audioSession)
    { }
    public AudioSessionControl(IAudioSessionControl2 audioSession)
    {
        _audioSession = audioSession;

        Debug.Assert(audioSession is ISimpleAudioVolume);
        SimpleAudioVolume = new SimpleAudioVolume((ISimpleAudioVolume)audioSession);
    }

    public void RegisterEventClient(IAudioSessionEventsHandler eventClient)
    {
        UnRegisterEventClient();

        _eventsCallback = new AudioSessionEventsCallback(eventClient);
        _audioSession.RegisterAudioSessionNotification(_eventsCallback);
    }

    public void UnRegisterEventClient()
    {
        if (_eventsCallback is not null)
        {
            _audioSession.UnregisterAudioSessionNotification(_eventsCallback);

            _eventsCallback = null;
        }
    }

    public void Dispose()
    {
        UnRegisterEventClient();
        GC.SuppressFinalize(this);
    }
}
