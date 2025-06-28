namespace AudioLocker.Core.CoreAudioAPI.Enums;

// https://learn.microsoft.com/en-us/windows/win32/api/mmdeviceapi/ne-mmdeviceapi-edataflow
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\mmdeviceapi.idl"
public enum EDataFlow
{
    eRender,
    eCapture,
    eAll,
    EDataFlow_enum_count
}
