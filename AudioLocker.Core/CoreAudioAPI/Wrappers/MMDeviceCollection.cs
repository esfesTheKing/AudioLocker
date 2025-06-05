using AudioLocker.Core.CoreAudioAPI.Interfaces;
using System.Collections;

namespace AudioLocker.Core.CoreAudioAPI.Wrappers;

public class MMDeviceCollection(IMMDeviceCollection collection) : IEnumerable<MMDevice>
{
    private readonly IMMDeviceCollection _collection = collection;

    public int Count
    {
        get => _collection.GetCount();
    }

    public MMDevice this[int index]
    {
        get
        {
            IMMDevice device = _collection.Item(index);

            return new MMDevice(device);
        }
    }

    public IEnumerator<MMDevice> GetEnumerator()
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
