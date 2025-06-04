namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;

public enum AudioSessionDisconnectReason
{
    DisconnectReasonDeviceRemoval,
    DisconnectReasonServerShutdown,
    DisconnectReasonFormatChanged,
    DisconnectReasonSessionLogoff,
    DisconnectReasonSessionDisconnected,
    DisconnectReasonExclusiveModeOverride
}
