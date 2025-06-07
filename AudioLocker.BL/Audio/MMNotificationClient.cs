using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Structs;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.Loggers.Abstract;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.BL.Audio;

[GeneratedComClass]
public partial class MMNotificationClient : IMMNotificationClient
{
    private readonly MMDeviceEnumerator _enumerator;
    private readonly AudioManager _audioManager;

    private readonly ILogger _logger;

    public MMNotificationClient(ILogger logger, MMDeviceEnumerator enumerator, AudioManager audioManager)
    {
        _logger = logger;
        _enumerator = enumerator;
        _audioManager = audioManager;
    }

    public void OnDeviceStateChanged(string deviceId, DeviceState newState)
    {
        var device = _enumerator.GetDevice(deviceId);
        if (!IsSupportedDevice(device))
        {
            return;
        }

        if (newState == DeviceState.DEVICE_STATE_ACTIVE)
        {
            _logger.Info($"Device's state was set to active: {device.FriendlyName}");

            OnDeviceAdded(deviceId);
        }
    }

    private bool IsSupportedDevice(MMDevice device)
    {
        return device.DataFlow == EDataFlow.eRender;
    }

    public void OnDeviceAdded(string pwstrDeviceId)
    {
        var device = _enumerator.GetDevice(pwstrDeviceId);
        if (!IsSupportedDevice(device))
        {
            return;
        }

        _logger.Info($"New device was connected: {device.FriendlyName}");

        Task.Run(async () => await _audioManager.SetupMMDevice(device));
    }

    public void OnDeviceRemoved(string deviceId)
    {
        var device = _enumerator.GetDevice(deviceId);
        if (!IsSupportedDevice(device))
        {
            return;
        }

        _logger.Info($"Device has disconnected: {device.FriendlyName}");
        _audioManager.RemoveSessionHandlers(device);
        //Task.Run(() => _audioManager.RemoveSessionHandlers(device));
    }

    public void OnDefaultDeviceChanged(EDataFlow dataFlow, ERole role, string defaultDeviceId)
    {
    }

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
    {
    }
}
