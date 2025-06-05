using AudioLocker.Core.CoreAudioAPI.Interfaces;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class SimpleAudioVolume(ISimpleAudioVolume simpleAudioVolume)
{
    private ISimpleAudioVolume _simpleAudioVolume = simpleAudioVolume;

    public float Volume
    {
        get => _simpleAudioVolume.GetMasterVolume();
        set => SetVolume(value);
    }


    public bool Mute
    {
        get => _simpleAudioVolume.GetMute();
        set => _simpleAudioVolume.SetMute(value, Guid.Empty);
    }

    public void SetVolume(float level)
    {
        if (0 <= level && level <= 1)
        {
            _simpleAudioVolume.SetMasterVolume(level, Guid.Empty);
        }
    }
}
