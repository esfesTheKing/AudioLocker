using AudioLocker.Common.DataTypes;
using System.Text.Json.Serialization;

namespace AudioLocker.BL.Configuration;

public class GeneralAudioConfiguration : Dictionary<string, DeviceAudioConfiguration> 
{ }

[JsonSourceGenerationOptions(WriteIndented = true, IndentSize = 4)]
[JsonSerializable(typeof(GeneralAudioConfiguration))]
internal partial class GeneralAudioConfigurationSerializationContext : JsonSerializerContext
{ }
