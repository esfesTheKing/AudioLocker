using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.Loggers.Abstract;
using NAudio.CoreAudioApi;
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
            () => { },
            exception =>
            {
                _logger.Warning("Unknown error has accord while trying to configure new session: ", exception);
            },
            () => { }
        );
    }

    public async Task Initialize()
    {
        foreach (var device in _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
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

        for (int i = 0; i < sessions.Count; ++i)
        {
            var session = sessions[i];
            if (session is null)
            {
                continue;
            }

            _comExceptionHandler.HandleSessionAccessExceptions(() => ConfigureSession(session, deviceName));
        }

        await _storage.Save();

        device.AudioSessionManager.OnSessionCreated += (_, newSession) =>
        {
            var session = new AudioSessionControl(newSession);

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
        var process = Process.GetProcessById((int)session.GetProcessID);
        var processName = process.ProcessName;

        _storage.Register(deviceName, processName);

        var audioSessionEventHandler = new AudioSessionEventHandler(_logger, _storage, session, deviceName, processName);

        var simpleAudioVolumeInterface = session.SimpleAudioVolume;
        audioSessionEventHandler.OnVolumeChanged(simpleAudioVolumeInterface.Volume, simpleAudioVolumeInterface.Mute);

        session.RegisterEventClient(audioSessionEventHandler);
        _logger.Info($"New session was configured: {processName}");
    }
}
