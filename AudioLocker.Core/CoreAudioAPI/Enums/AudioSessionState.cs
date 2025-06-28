namespace AudioLocker.Core.CoreAudioAPI.Enums;

// https://learn.microsoft.com/en-us/windows/win32/api/audiosessiontypes/ne-audiosessiontypes-audiosessionstate
// C Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\AudioSessionTypes.h"
public enum AudioSessionState
{
    AudioSessionStateInactive = 0,
    AudioSessionStateActive = 1,
    AudioSessionStateExpired = 2
}