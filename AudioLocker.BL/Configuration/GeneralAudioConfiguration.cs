using AudioLocker.Common.DataTypes;
using System.Text.Json.Serialization;

namespace AudioLocker.BL.ConfigurationStorage;

public class GeneralAudioConfiguration : Dictionary<string, DeviceAudioConfiguration>
{
    internal AlternateLookup<ReadOnlySpan<char>> _cache;

    public GeneralAudioConfiguration()
    {
        _cache = GetAlternateLookup<ReadOnlySpan<char>>();
    }

    public bool TryGetValue(ReadOnlySpan<char> key, out DeviceAudioConfiguration? value) => _cache.TryGetValue(key, out value);
}

[JsonSourceGenerationOptions(WriteIndented = true, IndentSize = 4)]
[JsonSerializable(typeof(GeneralAudioConfiguration))]
internal partial class GeneralAudioConfigurationSerializationContext : JsonSerializerContext
{ }
