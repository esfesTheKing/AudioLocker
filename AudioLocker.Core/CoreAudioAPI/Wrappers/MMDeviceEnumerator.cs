using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.PInvoke;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
public class MMDeviceEnumerator
{
    private readonly IMMDeviceEnumerator _enumerator;

    public MMDeviceEnumerator()
    {
        ComWrappers cw = new StrategyBasedComWrappers();

        var mmDeviceEnumeratorGuid = typeof(MMDeviceEnumerator).GUID;
        var iMMDeviceEnumeratorGuid = typeof(IMMDeviceEnumerator).GUID;

        Marshal.ThrowExceptionForHR(
            Ole32.CoCreateInstance(
                ref mmDeviceEnumeratorGuid,
                IntPtr.Zero,
                (int)CLSCTX.CLSCTX_ALL,
                ref iMMDeviceEnumeratorGuid,
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

    public void RegisterNotificationCallback(IMMNotificationClient notificationClient)
    {
        _enumerator.RegisterEndpointNotificationCallback(notificationClient);
    }

    public void UnregisterNotificationCallback(IMMNotificationClient notificationClient)
    {
        _enumerator.UnregisterEndpointNotificationCallback(notificationClient);
    }
}
