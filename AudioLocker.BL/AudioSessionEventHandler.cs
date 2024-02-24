using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.Loggers.Abstract;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AudioLocker.BL;

public class AudioSessionEventHandler : IAudioSessionEventsHandler
{
    private const uint DEVICE_INVALIDATED_ERORR = 0x88890004;

    private readonly ILogger _logger;
    private readonly IConfigurationStorage _configurationStorage;
    private readonly AudioSessionControl _session;
    private readonly string _deviceName;
    private readonly string _processName;

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
    }

    public void OnStateChanged(AudioSessionState state)
    {
        if (state == AudioSessionState.AudioSessionStateExpired)
        {
            _logger.Info($"{_processName}: application closed");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetVolumeLevel(float volume) => (int)(volume * 100);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetVolumeLevelPercentage(int volume) => (float)volume / 100;

    private void OnConfigurationChanged()
    {
        HandleSessionAccessExceptions(() => OnVolumeChangedImplementation(_session.SimpleAudioVolume.Volume));
    }

    public void OnVolumeChanged(float volume, bool isMuted)
    {
        HandleSessionAccessExceptions(() => OnVolumeChangedImplementation(volume));
    }

    private void OnVolumeChangedImplementation(float volume)
    {
        var configuration = _configurationStorage.Get(_deviceName, _processName);
        if (configuration is null)
        {
            return;
        }

        if (configuration.IsManual)
        {
            return;
        }

        if (configuration.VolumeLevel == GetVolumeLevel(volume))
        {
            return;
        }

        _session.SimpleAudioVolume.Volume = GetVolumeLevelPercentage(configuration.VolumeLevel);
        _logger.Info($"{_processName}: volume level was set from {GetVolumeLevel(volume)} to {configuration.VolumeLevel}");
    }

    public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
    {
        Console.WriteLine($"Session disconnected: {_processName}");
    }

    public void OnDisplayNameChanged(string displayName)
    {
    }

    public void OnChannelVolumeChanged(uint channelCount, nint newVolumes, uint channelIndex)
    {
    }

    public void OnGroupingParamChanged(ref Guid groupingId)
    {
    }

    public void OnIconPathChanged(string iconPath)
    {
    }
    private void HandleSessionAccessExceptions(Action function)
    {
        try
        {
            function.Invoke();
        }
        catch (COMException e)
        {
            var statusCode = unchecked((uint)e.ErrorCode);
            if (statusCode != DEVICE_INVALIDATED_ERORR)
            {
                _logger.Warning($"Unknown error encountered: {_deviceName} - {_processName} - {statusCode}", e);
            }

            _logger.Info($"Unergistering event handler for {_deviceName} - {_processName}");

            _session.UnRegisterEventClient(this);
        }
    }
}
