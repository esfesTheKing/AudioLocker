using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Structs;
using System.Runtime.InteropServices;


namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public partial class MMDevice
{
    private readonly IMMDevice _device;
    private readonly IPropertyStore _propertyStore;

    public readonly string Id;

    public readonly AudioSessionManager AudioSessionManager;

    public MMDevice(IMMDevice device)
    {
        _device = device;
        _propertyStore = _device.OpenPropertyStore(STGM.STGM_READ);

        Id = _device.GetId();

        var sessionManager = _device.Activate<IAudioSessionManager2>();

        AudioSessionManager = new AudioSessionManager(sessionManager);
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

    public EDataFlow DataFlow
    {
        get => ((IMMEndpoint)_device).GetDataFlow();
    }

    private string GetPropertyValue(PropertyKey key)
    {
        var value = _propertyStore.GetValue(ref key);

        return Marshal.PtrToStringAuto(value.pwszVal) ?? "Unknown";
    }
}
