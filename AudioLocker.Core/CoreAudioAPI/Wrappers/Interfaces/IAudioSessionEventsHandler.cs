using AudioLocker.Core.CoreAudioAPI.Enums;
namespace AudioLocker.Core.CoreAudioAPI.Wrappers.Interfaces;

public interface IAudioSessionEventsHandler
{
    void OnChannelVolumeChanged(uint ChannelCount, nint NewChannelVolumeArray, uint ChangedChannel);

    void OnDisplayNameChanged(string NewDisplayName);

    void OnGroupingParamChanged(ref Guid NewGroupingParam);

    void OnIconPathChanged(string NewIconPath);

    void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);

    void OnVolumeChanged(float NewVolume, bool NewMute);

    void OnStateChanged(AudioSessionState NewState);
}
