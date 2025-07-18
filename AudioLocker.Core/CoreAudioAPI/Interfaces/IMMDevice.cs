﻿using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/mmdeviceapi/nn-mmdeviceapi-immdevice
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\mmdeviceapi.idl"
[GeneratedComInterface]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
public partial interface IMMDevice
{
    [return: MarshalAs(UnmanagedType.Interface)]
    object Activate(ref Guid iid, CLSCTX dwClsCtx, IntPtr pActivationParams);

    IPropertyStore OpenPropertyStore(STGM stgmAccess);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetId();

    DeviceState GetState();
}

public static class IMMDeviceExtension
{
    public static T Activate<T>(this IMMDevice device)
    {
        var iid = typeof(T).GUID;
        var obj = device.Activate(ref iid, CLSCTX.CLSCTX_ALL, IntPtr.Zero);

        return (T)obj;
    }
}
