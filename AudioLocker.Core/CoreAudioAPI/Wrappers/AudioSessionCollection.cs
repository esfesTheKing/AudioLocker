using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Collections;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionCollection : ICollection<AudioSessionControl>
{
    private readonly IAudioSessionEnumerator _enumerator;
    private readonly List<AudioSessionControl> _sessions;

    public AudioSessionCollection(IAudioSessionEnumerator enumerator)
    {
        _enumerator = enumerator;

        _sessions = GetInitialSessions();
    }

    public int Count
    {
        get => _sessions.Count;
    }

    public bool IsReadOnly => false;

    public AudioSessionControl this[int index]
    {
        get => _sessions[index];
    }

    private List<AudioSessionControl> GetInitialSessions()
    {
        var sessions = new List<AudioSessionControl>();

        for (int index = 0; index < _enumerator.GetCount(); index++)
        {
            IAudioSessionControl sessionInterface = _enumerator.GetSession(index);
            var session = new AudioSessionControl(sessionInterface);

            session.OnSessionDisconnect += OnSessionDisconnect;

            sessions.Add(session);
        }

        return sessions;
    }

    private void OnSessionDisconnect(object sender) => Remove((AudioSessionControl)sender);

    public void Add(AudioSessionControl item) 
    {
        item.OnSessionDisconnect += OnSessionDisconnect;

        _sessions.Add(item);
    }

    public void Clear() => _sessions.Clear();

    public bool Contains(AudioSessionControl item) => _sessions.Contains(item);

    public void CopyTo(AudioSessionControl[] array, int arrayIndex) => _sessions.CopyTo(array, arrayIndex);

    public bool Remove(AudioSessionControl item) => _sessions.Remove(item);

    public IEnumerator<AudioSessionControl> GetEnumerator()
    {
        for (int index = 0; index < Count; index++)
        {
            yield return this[index];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
