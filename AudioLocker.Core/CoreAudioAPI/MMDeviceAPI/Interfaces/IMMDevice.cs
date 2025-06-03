using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
public partial interface IMMDevice
{
    void Activate(ref Guid interfaceId, ClsCtx clsCtx, PROPVARIANT activationParams, out IntPtr interfacePointer);
    void GetId([MarshalAs(UnmanagedType.LPWStr)] out string id);
    void GetState(out DeviceState state);
    void OpenPropertyStore(STGM stgmAccess, IPropertyStore store);
}
