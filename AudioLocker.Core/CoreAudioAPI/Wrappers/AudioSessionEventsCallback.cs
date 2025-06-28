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

    public void OnDisplayNameChanged(string NewDisplayName, Guid EventContext)
    {
        _handler.OnDisplayNameChanged(NewDisplayName);
    }

    public void OnIconPathChanged(string NewIconPath, Guid EventContext)
    {
        _handler.OnIconPathChanged(NewIconPath);
    }

    public void OnSimpleVolumeChanged(float NewVolume, [MarshalAs(UnmanagedType.Bool)] bool NewMute, Guid EventContext)
    {
        _handler.OnVolumeChanged(NewVolume, NewMute);
    }

    public void OnChannelVolumeChanged(uint ChannelCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] float[] NewChannelVolumeArray, uint ChangedChannel, Guid EventContext)
    {
        _handler.OnChannelVolumeChanged(ChannelCount, NewChannelVolumeArray, ChangedChannel);
    }

    public void OnGroupingParamChanged(Guid NewGroupingParam, Guid EventContext)
    {
        _handler.OnGroupingParamChanged(NewGroupingParam);
    }

    public void OnStateChanged(AudioSessionState NewState)
    {
        if (NewState == AudioSessionState.AudioSessionStateExpired)
        {
            OnSessionDisconnect?.Invoke();
        }

        _handler.OnStateChanged(NewState);
    }

    public void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason)
    {
        OnSessionDisconnect?.Invoke();

        _handler.OnSessionDisconnected(DisconnectReason);
    }
}
