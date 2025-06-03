using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;

//[GeneratedComClass]
//[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
//public partial class MMDeviceEnumeratorCOM
//{

//}

public partial class MMDeviceEnumerator
{
    [LibraryImport("Ole32")]
    private static partial int CoCreateInstance(Guid rclsid, IntPtr pUnkOuter, int dwClsContext, Guid riid, out IntPtr ppObj);

    private readonly IMMDeviceEnumerator _enumerator;

    public MMDeviceEnumerator()
    {
        ComWrappers cw = new StrategyBasedComWrappers();

        Marshal.ThrowExceptionForHR(
            CoCreateInstance(
                new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E"), 
                IntPtr.Zero, 
                (int)ClsCtx.INPROC_SERVER, 
                new Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), 
                out IntPtr obj
            )
        );

        _enumerator = (IMMDeviceEnumerator)cw.GetOrCreateObjectForComInstance(obj, CreateObjectFlags.None);
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

