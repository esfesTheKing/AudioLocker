using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Structs;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.Loggers.Abstract;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.BL.Audio;

[GeneratedComClass]
public partial class MMNotificationClient(
        ILogger logger, 
        MMDeviceEnumerator enumerator, 
        DeviceConfigurationHandler deviceConfigurationHandler
    ) : IMMNotificationClient
{
    private readonly MMDeviceEnumerator _enumerator = enumerator;
    private readonly DeviceConfigurationHandler _deviceConfigurationHandler = deviceConfigurationHandler;

    private readonly ILogger _logger = logger;

    public void OnDeviceStateChanged(string deviceId, DeviceState newState)
    {
        var device = _enumerator.GetDevice(deviceId);
        if (!IsSupportedDevice(device))
        {
            return;
        }

        switch (newState)
        {
            case DeviceState.DEVICE_STATE_ACTIVE:
                _logger.Info($"[{device.FriendlyName}]: Device's state was set to active");
                OnDeviceAdded(deviceId);
                break;
            case DeviceState.DEVICE_STATE_DISABLED:
            case DeviceState.DEVICE_STATE_UNPLUGGED:
            case DeviceState.DEVICE_STATE_NOTPRESENT:
                _logger.Info($"[{device.FriendlyName}]: Device's state was set to inactive");
                OnDeviceRemoved(deviceId);
                break;
        }
    }

    public void OnDeviceAdded(string pwstrDeviceId)
    {
        var device = _enumerator.GetDevice(pwstrDeviceId);
        if (!IsSupportedDevice(device))
        {
            return;
        }

        _logger.Info($"[{device.FriendlyName}]: New device was connected");
        _deviceConfigurationHandler.ConfigureDevice(device);
    }

    public void OnDeviceRemoved(string deviceId)
    {
        var device = _enumerator.GetDevice(deviceId);
        if (!IsSupportedDevice(device))
        {
            return;
        }

        _logger.Info($"[{device.FriendlyName}]: Device has disconnected");
        _deviceConfigurationHandler.DeconfigureDevice(device);
    }

    public void OnDefaultDeviceChanged(EDataFlow dataFlow, ERole role, string defaultDeviceId) { }

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSupportedDevice(MMDevice device) => device.DataFlow == EDataFlow.eRender;
}
