namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;

// https://learn.microsoft.com/en-us/windows/win32/coreaudio/device-state-xxx-constants
[Flags]
public enum DeviceState
{
    DEVICE_STATE_ACTIVE = 0x1,
    DEVICE_STATE_DISABLED = 0x2,
    DEVICE_STATE_NOTPRESENT = 0x4,
    DEVICE_STATE_UNPLUGGED = 0x8,
    DEVICE_STATEMASK_ALL = 0xF
}
