namespace AudioLocker.Core.CoreAudioAPI.Enums;

// https://learn.microsoft.com/en-us/windows/win32/api/mmdeviceapi/ne-mmdeviceapi-edataflow
public enum EDataFlow
{
    eRender = 0,
    eCapture,
    eAll,
    EDataFlow_enum_count
}
