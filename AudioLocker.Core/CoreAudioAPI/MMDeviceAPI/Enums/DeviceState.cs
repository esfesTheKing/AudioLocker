namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;

[Flags]
public enum DeviceState
{
    Active = 1,
    Disabled = 2,
    NotPresent = 4,
    Unplugged = 8,
    All = 0xF
}
