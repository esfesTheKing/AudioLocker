using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.Loggers.Abstract;

using System.Diagnostics;

namespace AudioLocker.BL.Audio;

public class AudioManager
{
    private readonly MMDeviceEnumerator _enumerator;
    private readonly ILogger _logger;
    private readonly IConfigurationStorage _storage;

    private readonly COMExceptionHandler _comExceptionHandler;

    public AudioManager(ILogger logger, IConfigurationStorage storage, MMDeviceEnumerator enumerator)
    {
        _logger = logger;
        _enumerator = enumerator;
        _storage = storage;

        _comExceptionHandler = new COMExceptionHandler(
            onKnownException: () => { },
            onUnknownException: exception =>
            {
                _logger.Warning("Unknown error has accord while trying to configure new session: ", exception);
            },
            onCleanup: () => { }
        );
    }

    public async Task Initialize()
    {
        foreach (var device in _enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.DEVICE_STATE_ACTIVE))
        {
            await SetupMMDevice(device);
        }
    }

    public void RemoveSessionHandlers(MMDevice device)
    {
        var sessions = device.AudioSessionManager.Sessions;

        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            session.Dispose();
        }
    }

    public async Task SetupMMDevice(MMDevice device)
    {
        var deviceName = device.FriendlyName;

        _logger.Info($"Configuring: {deviceName}");

        var sessions = device.AudioSessionManager.Sessions;

        foreach (var session in sessions)
        {
            _comExceptionHandler.HandleSessionAccessExceptions(() => ConfigureSession(session, deviceName));
        }

        await _storage.Save();

        device.AudioSessionManager.OnSessionCreated += (_, session) =>
        {
            _comExceptionHandler.HandleSessionAccessExceptionsAsync(async () =>
            {
                ConfigureSession(session, deviceName);

                await _storage.Save();
            });
        };

        _logger.Info($"Finished configuring: {deviceName}");
    }

    private void ConfigureSession(AudioSessionControl session, string deviceName)
    {
        var process = Process.GetProcessById((int)session.ProcessId);
        var processName = process.ProcessName;

        _logger.Debug($"{processName} - {(int)session.ProcessId}");

        _storage.Register(deviceName, processName);

        var audioSessionEventHandler = new AudioSessionEventHandler(_logger, _storage, session, deviceName, processName);

        var simpleAudioVolume = session.SimpleAudioVolume;
        audioSessionEventHandler.OnVolumeChanged(simpleAudioVolume.Volume, simpleAudioVolume.Mute);

        session.RegisterEventClient(audioSessionEventHandler);
        _logger.Info($"New session was configured: {processName}");
    }
}
