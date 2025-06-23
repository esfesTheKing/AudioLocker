using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using AudioLocker.Core.CoreAudioAPI.Wrappers.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

[GeneratedComClass]
public partial class AudioSessionEventsCallback(IAudioSessionEventsHandler handler) : IAudioSessionEvents
{
    private readonly IAudioSessionEventsHandler _handler = handler;

    internal event Action? OnSessionDisconnect;

    public void OnChannelVolumeChanged(uint ChannelCount, nint NewChannelVolumeArray, uint ChangedChannel, ref Guid EventContext)
    {
        _handler.OnChannelVolumeChanged(ChannelCount, NewChannelVolumeArray, ChangedChannel);
    }

    public void OnDisplayNameChanged(string NewDisplayName, ref Guid EventContext)
    {
        _handler.OnDisplayNameChanged(NewDisplayName);
    }

    public void OnGroupingParamChanged(ref Guid NewGroupingParam, ref Guid EventContext)
    {
        _handler.OnGroupingParamChanged(ref NewGroupingParam);
    }

    public void OnIconPathChanged(string NewIconPath, ref Guid EventContext)
    {
        _handler.OnIconPathChanged(NewIconPath);
    }

    public void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason)
    {
        OnSessionDisconnect?.Invoke();

        _handler.OnSessionDisconnected(DisconnectReason);
    }

    public void OnSimpleVolumeChanged(float NewVolume, [MarshalAs(UnmanagedType.Bool)] bool NewMute, ref Guid EventContext)
    {
        _handler.OnVolumeChanged(NewVolume, NewMute);
    }

    public void OnStateChanged(AudioSessionState NewState)
    {
        if (NewState == AudioSessionState.AudioSessionStateExpired)
        {
            OnSessionDisconnect?.Invoke();
        }

        _handler.OnStateChanged(NewState);
    }
}
