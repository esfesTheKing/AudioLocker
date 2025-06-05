using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;


[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
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
                typeof(MMDeviceEnumerator).GUID, 
                IntPtr.Zero, 
                (int)CLSCTX.CLSCTX_ALL, 
                typeof(IMMDeviceEnumerator).GUID, 
                out IntPtr obj
            )
        );

        _enumerator = (IMMDeviceEnumerator)cw.GetOrCreateObjectForComInstance(obj, CreateObjectFlags.None);
    }

    public MMDevice GetDevice(string id)
    {
        IMMDevice device = _enumerator.GetDevice(id);

        return new MMDevice(device);
    }

    public MMDeviceCollection EnumerateAudioEndPoints(EDataFlow dataFlow, DeviceState deviceState)
    {
        IMMDeviceCollection deviceCollection = _enumerator.EnumAudioEndpoints(dataFlow, deviceState);

        return new MMDeviceCollection(deviceCollection);
    }

    public void RegisterEndpointNotificationCallback(IMMNotificationClient notificationClient)
    {
        _enumerator.RegisterEndpointNotificationCallback(notificationClient);
    }
}

