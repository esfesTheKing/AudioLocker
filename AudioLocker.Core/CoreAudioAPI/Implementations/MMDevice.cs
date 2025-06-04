using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Structs;
using System.Runtime.InteropServices;


namespace AudioLocker.Core.CoreAudioAPI.Implementations;

public partial class MMDevice
{
    private readonly IMMDevice _device;
    private readonly IPropertyStore _propertyStore;

    public readonly IAudioSessionManager2 AudioSessionManager;

    public MMDevice(IMMDevice device)
    {
        _device = device;
        _propertyStore = _device.OpenPropertyStore(STGM.STGM_READ);

        AudioSessionManager = _device.Activate<IAudioSessionManager2>();
    }

    public string Id
    {
        get => _device.GetId();
    }

    public DeviceState State
    {
        get => _device.GetState();
    }

    public string FriendlyName
    {
        get => GetPropertyValue(PropertyKeys.PKEY_Device_FriendlyName);
    }

    public string DeviceFriendlyName
    {
        get => GetPropertyValue(PropertyKeys.PKEY_DeviceInterface_FriendlyName);
    }

    private string GetPropertyValue(PropertyKey key)
    {
        var value = _propertyStore.GetValue(ref key);

        return Marshal.PtrToStringAuto(value.pwszVal) ?? "Unknown";
    }
}
