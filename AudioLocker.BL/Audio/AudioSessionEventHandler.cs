using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.CoreAudioAPI.Wrappers.Interfaces;
using AudioLocker.Core.Loggers.Abstract;
using System.Runtime.CompilerServices;

namespace AudioLocker.BL.Audio;

public class AudioSessionEventHandler : IAudioSessionEventsHandler, IDisposable
{
    private const int ON_VOLUME_CHANGE_DEBOUNCER_TIME_MS = 10;

    private readonly ILogger _logger;
    private readonly IConfigurationStorage _configurationStorage;
    private readonly AudioSessionControl _session;
    private readonly string _deviceName;
    private readonly string _processName;
    private readonly COMExceptionHandler _comExceptionHandler;

    private CancellationTokenSource? _token;

    public AudioSessionEventHandler(
            ILogger logger,
            IConfigurationStorage configurationStorage,
            AudioSessionControl session,
            string deviceName,
            string processName
        )
    {
        _logger = logger;
        _configurationStorage = configurationStorage;
        _session = session;
        _processName = processName;
        _deviceName = deviceName;

        _configurationStorage.OnConfigurationChanged += OnConfigurationChanged;

        _comExceptionHandler = new COMExceptionHandler(
            onKnownException: () => { },
            onUnknownException: exception => _logger.Warning($"Unknown error encountered: {_deviceName} - {_processName}", exception),
            onCleanup: Dispose
        );
    }

    public void OnStateChanged(AudioSessionState state)
    {
        if (state == AudioSessionState.AudioSessionStateExpired)
        {
            _logger.Info($"{_processName}: application closed");
            Dispose();
        }
    }

    public void OnVolumeChanged(float volume, bool isMuted)
    {
        _comExceptionHandler.HandleSessionAccessExceptions(() =>
        {
            _token?.Cancel();
            _token = new CancellationTokenSource();

            Task.Delay(ON_VOLUME_CHANGE_DEBOUNCER_TIME_MS, _token.Token).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    OnVolumeChanged(volume);
                }
            });
        });
    }

    public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
    {
        _logger.Info($"[{_deviceName}] {_processName}: Session disconnected");
        Dispose();
    }

    public void Dispose()
    {
        _logger.Info($"Unergistering event handler for {_deviceName} - {_processName}");

        _configurationStorage.OnConfigurationChanged -= OnConfigurationChanged;

        _session.Dispose();

        GC.SuppressFinalize(this);
    }

    internal void OnConfigurationChanged()
    {
        _comExceptionHandler.HandleSessionAccessExceptions(() => OnVolumeChanged(_session.SimpleAudioVolume.Volume));
    }

    private void OnVolumeChanged(float volume)
    {
        var configuration = _configurationStorage.Get(_deviceName, _processName);
        if (configuration?.IsManual ?? true)
        {
            return;
        }

        if (configuration.VolumeLevel == GetVolumeLevel(volume))
        {
            return;
        }

        _session.SimpleAudioVolume.Volume = GetVolumeLevelPercentage(configuration.VolumeLevel);
        _logger.Info($"[{_deviceName}] {_processName}: volume level was set from {GetVolumeLevel(volume)} to {configuration.VolumeLevel}");
    }

    public void OnDisplayNameChanged(string displayName) { }

    public void OnChannelVolumeChanged(uint channelCount, nint newVolumes, uint channelIndex) { }

    public void OnGroupingParamChanged(ref Guid groupingId) { }

    public void OnIconPathChanged(string iconPath) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetVolumeLevel(float volume) => (int)(volume * 100);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetVolumeLevelPercentage(int volume) => (float)volume / 100;
}
