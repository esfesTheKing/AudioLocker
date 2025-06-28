namespace AudioLocker.Core.CoreAudioAPI.Enums;

// https://learn.microsoft.com/en-us/windows/win32/api/mmdeviceapi/nf-mmdeviceapi-immdevice-openpropertystore
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\mmdeviceapi.idl"
public enum STGM
{
    STGM_READ,
    STGM_WRITE,
    STGM_READWRITE
}
