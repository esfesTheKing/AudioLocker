namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;

// https://learn.microsoft.com/en-us/windows/win32/api/audiosessiontypes/ne-audiosessiontypes-audiosessionstate
public enum AudioSessionState
{
    AudioSessionStateInactive,
    AudioSessionStateActive,
    AudioSessionStateExpired
}