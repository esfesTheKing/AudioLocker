using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;

public partial class MMDevice(IMMDevice device)
{
    private readonly IMMDevice _device = device;

    public string Id
    {
        get
        {
            _device.GetId(out string id);

            return id;
        }
    }

    public DeviceState State
    {
        get
        {
            _device.GetState(out DeviceState state);

            return state;
        }
    }
}
