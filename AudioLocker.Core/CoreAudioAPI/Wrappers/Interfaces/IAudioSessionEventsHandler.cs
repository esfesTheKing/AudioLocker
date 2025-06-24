using AudioLocker.Core.CoreAudioAPI.Enums;
namespace AudioLocker.Core.CoreAudioAPI.Wrappers.Interfaces;

public interface IAudioSessionEventsHandler
{
    void OnDisplayNameChanged(string NewDisplayName);

    void OnIconPathChanged(string NewIconPath);

    void OnVolumeChanged(float NewVolume, bool NewMute);

    void OnChannelVolumeChanged(uint ChannelCount, float[] NewChannelVolumeArray, uint ChangedChannel);

    void OnGroupingParamChanged(Guid NewGroupingParam);

    void OnStateChanged(AudioSessionState NewState);

    void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);
}
