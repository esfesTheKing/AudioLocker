using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;

[GeneratedComClass]
[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
public partial class MMDeviceEnumeratorCOM
{

}

public partial class MMDeviceEnumerator
{
    private IMMDeviceEnumerator _enumerator;

    public MMDeviceEnumerator()
    {
        _enumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
    }

    public MMDevice GetDevice(string id)
    {
        _enumerator.GetDevice(id, out IMMDevice device);

        return new MMDevice(device);
    }

    public MMDeviceCollection EnumerateAudioEndPoints(EDataFlow dataFlow, DeviceState deviceState)
    {
        _enumerator.EnumAudioEndpoints(dataFlow, deviceState, out IMMDeviceCollection deviceCollection);

        return new MMDeviceCollection(deviceCollection);
    }
}

