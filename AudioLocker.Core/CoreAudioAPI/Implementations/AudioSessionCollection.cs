using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Collections;

namespace AudioLocker.Core.CoreAudioAPI.Implementations;

public class AudioSessionCollection(IAudioSessionEnumerator enumerator) : IEnumerable<IAudioSessionControl>
{
    private readonly IAudioSessionEnumerator _enumerator = enumerator;

    public int Count
    {
        get => _enumerator.GetCount();
    }

    public IAudioSessionControl this[int index]
    {
        get
        {
            IAudioSessionControl session = _enumerator.GetSession(index);

            return session;
        }
    }

    public IEnumerator<IAudioSessionControl> GetEnumerator()
    {
        for (int index = 0; index < Count; index++)
        {
            yield return this[index];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
