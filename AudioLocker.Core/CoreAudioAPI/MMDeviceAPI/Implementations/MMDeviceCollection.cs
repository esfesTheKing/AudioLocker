using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;
using System.Collections;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;

public class MMDeviceCollection(IMMDeviceCollection collection) : IEnumerable<MMDevice>
{
    private readonly IMMDeviceCollection _collection = collection;

    public int Count
    {
        get
        {
            _collection.GetCount(out int deviceCount);

            return deviceCount;
        }
    }

    public MMDevice this[int index]
    {
        get
        {
            _collection.Item(index, out IMMDevice device);

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
