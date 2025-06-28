using AudioLocker.Core.ConfigurationStorage.Abstract;
using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.Loggers.Abstract;
using System.Diagnostics;

namespace AudioLocker.BL.Audio;

public class DeviceConfigurator : IDisposable
{
    private readonly ILogger _logger;
    private readonly IConfigurationStorage _storage;
    private readonly MMDeviceEnumerator _enumerator;

    private MMNotificationClient? _notificationClient;
    private readonly Dictionary<string, MMDevice> _configuredDevices = [];

    private readonly COMExceptionHandler _comExceptionHandler;

    public DeviceConfigurator(ILogger logger, IConfigurationStorage storage, MMDeviceEnumerator enumerator)
    {
        _logger = logger;
        _storage = storage;
        _enumerator = enumerator;

        _comExceptionHandler = new COMExceptionHandler(
            _logger,
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
            _comExceptionHandler.HandleAccessExceptions(() => ConfigureDevice(device));
        }

        _notificationClient = new MMNotificationClient(_logger, _enumerator);
        _notificationClient.OnDeviceAddedEvent += (device) => _comExceptionHandler.HandleAccessExceptions(() => ConfigureDevice(device));
        _notificationClient.OnDeviceRemovedEvent += (device) => _comExceptionHandler.HandleAccessExceptions(() => DeconfigureDevice(device));

        _enumerator.RegisterNotificationCallback(_notificationClient);
    }

    public void Dispose()
    {
        if (_notificationClient is not null)
        {
            _enumerator.UnregisterNotificationCallback(_notificationClient);
            _notificationClient.Dispose();
        }

        foreach (var device in _configuredDevices.Values)
        {
            _comExceptionHandler.HandleAccessExceptions(() => DeconfigureDevice(device));
        }

        GC.SuppressFinalize(this);
    }

    private void ConfigureDevice(MMDevice device)
    {
        var deviceName = device.FriendlyName;
        var audioSessionManager = device.AudioSessionManager;

        _logger.Info($"[{deviceName}]: Configuring device...");
        if (_configuredDevices.ContainsKey(device.Id))
        {
            _logger.Info($"[{deviceName}]: Device was configured previously...");
            return;
        }

        foreach (var session in audioSessionManager.Sessions)
        {
            _comExceptionHandler.HandleAccessExceptionsAsync(async () => await ConfigureSession(deviceName, session));
        }

        audioSessionManager.OnSessionCreated += (_, session) =>
        {
            _comExceptionHandler.HandleAccessExceptionsAsync(async () => await ConfigureSession(deviceName, session));
        };

        _configuredDevices.Add(device.Id, device);
        _logger.Info($"[{deviceName}]: Finished configuring device");
    }

    private void DeconfigureDevice(MMDevice device)
    {
        device.AudioSessionManager.Dispose();

        var deviceWasFound = _configuredDevices.Remove(device.Id);
        if (!deviceWasFound)
        {
            _logger.Warning($"[{device.FriendlyName}]: Was not found in _configuredDevices");
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
