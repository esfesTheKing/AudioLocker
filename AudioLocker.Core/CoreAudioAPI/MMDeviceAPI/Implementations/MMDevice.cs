using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Structs;
using System.Runtime.InteropServices;


namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;

public partial class MMDevice
{
    private readonly IMMDevice _device;
    private readonly IPropertyStore _propertyStore;

    public readonly IAudioSessionManager2 AudioSessionManager;

    public MMDevice(IMMDevice device)
    {
        _device = device;
        _propertyStore = _device.OpenPropertyStore(STGM.Read);

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

    private string GetPropertyValue(PropertyKey key)
    {
        var value = _propertyStore.GetValue(ref key);
        if (value.pwszVal == IntPtr.Zero)
        {
            return "Unknown";
        }

        return Marshal.PtrToStringAuto(value.pwszVal)!;
    }
}
