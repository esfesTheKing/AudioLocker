using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Collections;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class AudioSessionCollection : ICollection<AudioSessionControl>
{
    private readonly IAudioSessionEnumerator _enumerator;
    private readonly OrderedDictionary<int, AudioSessionControl> _sessions;

    public AudioSessionCollection(IAudioSessionEnumerator enumerator)
    {
        _enumerator = enumerator;

        _sessions = GetSessionsFromEnumerator();
    }

    public int Count
    {
        get => _sessions.Count;
    }

    public bool IsReadOnly => false;

    /// <summary>
    /// Tries to add the AudioSessionControl to the collections.
    /// </summary>
    /// <remarks>
    /// If unable to add `item` to the collection, this method will silently fail.
    /// </remarks>
    public void Add(AudioSessionControl item)
    {
        item.OnSessionDisconnect += OnSessionDisconnect;

        var success = _sessions.TryAdd(item.GetHashCode(), item);
        if (!success)
        {
            item.OnSessionDisconnect -= OnSessionDisconnect;
        }
    }

    public void Clear() => _sessions.Clear();

    public bool Contains(AudioSessionControl item) => _sessions.ContainsKey(item.GetHashCode());

    public void CopyTo(AudioSessionControl[] array, int arrayIndex) => _sessions.Values.CopyTo(array, arrayIndex);

    public bool Remove(AudioSessionControl item) => _sessions.Remove(item.GetHashCode());

    public IEnumerator<AudioSessionControl> GetEnumerator() => _sessions.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private OrderedDictionary<int, AudioSessionControl> GetSessionsFromEnumerator()
    {
        var sessions = new OrderedDictionary<int, AudioSessionControl>();

        for (int index = 0; index < _enumerator.GetCount(); index++)
        {
            IAudioSessionControl sessionInterface = _enumerator.GetSession(index);
            var session = new AudioSessionControl(sessionInterface);

            session.OnSessionDisconnect += OnSessionDisconnect;

            sessions.Add(session.GetHashCode(), session);
        }

        return sessions;
    }

    private void OnSessionDisconnect(object sender) => Remove((AudioSessionControl)sender);
}
