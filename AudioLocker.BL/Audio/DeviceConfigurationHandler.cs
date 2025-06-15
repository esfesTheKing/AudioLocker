using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.Loggers.Abstract;
using System.Diagnostics;

namespace AudioLocker.BL.Audio;

public class DeviceConfigurationHandler: IDisposable
{
    private readonly ILogger _logger;
    private readonly IConfigurationStorage _storage;
    private readonly MMDeviceEnumerator _enumerator;

    private IMMNotificationClient? _notificationClient;

    private readonly COMExceptionHandler _comExceptionHandler;

    public DeviceConfigurationHandler(ILogger logger, IConfigurationStorage storage, MMDeviceEnumerator enumerator)
    {
        _logger = logger;
        _storage = storage;
        _enumerator = enumerator;

        _comExceptionHandler = new COMExceptionHandler(
            onKnownException: () => { },
            onUnknownException: exception => _logger.Warning("Unknown error has accord while trying to configure new session: ", exception),
            onCleanup: () => { }
        );
    }

    public void Initialize()
    {
        var devices = _enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.DEVICE_STATE_ACTIVE);
        foreach (var device in devices)
        {
            _comExceptionHandler.HandleSessionAccessExceptions(() => ConfigureDevice(device));
        }

        _notificationClient = new MMNotificationClient(_logger, _enumerator, this);

        _enumerator.RegisterNotificationCallback(_notificationClient);
    }

    public void Dispose()
    {
        if (_notificationClient is not null)
        {
            _enumerator.UnregisterNotificationCallback(_notificationClient);
        }

        var devices = _enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.DEVICE_STATE_ACTIVE);
        foreach (var device in devices)
        {
            _comExceptionHandler.HandleSessionAccessExceptions(() => DeconfigureDevice(device));
        }

        GC.SuppressFinalize(this);
    }

    internal void ConfigureDevice(MMDevice device)
    {
        var deviceName = device.FriendlyName;
        var audioSessionManager = device.AudioSessionManager;

        _logger.Info($"[{deviceName}]: Configuring device...");

        foreach (var session in audioSessionManager.Sessions)
        {
            _comExceptionHandler.HandleSessionAccessExceptionsAsync(async () => await ConfigureSession(deviceName, session));
        }

        audioSessionManager.OnSessionCreated += (_, session) =>
        {
            _comExceptionHandler.HandleSessionAccessExceptionsAsync(async () => await ConfigureSession(deviceName, session));
        };

        _logger.Info($"[{deviceName}]: Finished configuring device");
    }

    internal void DeconfigureDevice(MMDevice device)
    {
        foreach (var session in device.AudioSessionManager.Sessions)
        {
            session.Dispose();
        }
    }

    private async Task ConfigureSession(string deviceName, AudioSessionControl session)
    {
        var process = Process.GetProcessById((int)session.ProcessId);

        _logger.Info($"[{deviceName}]: Configuring {process.ProcessName} - {process.Id}");

        _storage.Register(deviceName, process.ProcessName);

        var audioSessionEventHandler = new AudioSessionEventHandler(_logger, _storage, session, deviceName, process.ProcessName);
        audioSessionEventHandler.OnConfigurationChanged();

        session.RegisterEventClient(audioSessionEventHandler);
        await _storage.Save();

        _logger.Info($"[{deviceName}]: Finished configuring {process.ProcessName}");
    }
}
