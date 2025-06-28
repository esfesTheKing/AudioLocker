namespace AudioLocker.Core.CoreAudioAPI.Enums;

// https://learn.microsoft.com/en-us/windows/win32/api/audiopolicy/nf-audiopolicy-iaudiosessionevents-onsessiondisconnected
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\audiopolicy.idl"
public enum AudioSessionDisconnectReason
{
    DisconnectReasonDeviceRemoval,
    DisconnectReasonServerShutdown,
    DisconnectReasonFormatChanged,
    DisconnectReasonSessionLogoff,
    DisconnectReasonSessionDisconnected,
    DisconnectReasonExclusiveModeOverride
}
