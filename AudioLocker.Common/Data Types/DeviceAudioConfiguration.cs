namespace AudioLocker.Common.DataTypes;

public class DeviceAudioConfiguration : Dictionary<string, ProcessAudioConfiguration>
{
    internal AlternateLookup<ReadOnlySpan<char>> _cache;

    public DeviceAudioConfiguration()
    {
        _cache = GetAlternateLookup<ReadOnlySpan<char>>();
    }

    public bool TryGetValue(ReadOnlySpan<char> key, out ProcessAudioConfiguration? value) => _cache.TryGetValue(key, out value);
}
