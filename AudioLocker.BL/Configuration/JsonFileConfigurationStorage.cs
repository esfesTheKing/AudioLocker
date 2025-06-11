using AudioLocker.Common.DataTypes;
using AudioLocker.Common.Extenstions;
using AudioLocker.Core.ConfigurationStorage.Abstract;
using System.Text;
using System.Text.Json;

namespace AudioLocker.BL.Configuration;

public class JsonFileConfigurationStorage(string filePath, int defaultVolumeLevel) : FileConfigurationStorage<GeneralAudioConfiguration>(filePath)
{
    private static readonly GeneralAudioConfiguration EMPTY_GENERAL_AUDIO_CONFIGURATION = new GeneralAudioConfiguration();

    private GeneralAudioConfiguration _generalAudioConfiguration = new GeneralAudioConfiguration();
    private readonly int _defaultVolumeLevel = defaultVolumeLevel;
    private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

    public override ProcessAudioConfiguration? Get(string deviceName, string processName)
    {
        _generalAudioConfiguration.TryGetValue(deviceName, out var deviceConfiguration);
        if (deviceConfiguration is null)
        {
            return null;
        }

        deviceConfiguration.TryGetValue(processName, out var generalAudioConfiguration);

        return generalAudioConfiguration;
    }

    public override void Set(string deviceName, string processName, ProcessAudioConfiguration configuration)
    {
        _generalAudioConfiguration.TryGetValue(deviceName, out var deviceConfiguration);
        if (deviceConfiguration is null)
        {
            return;
        }

        deviceConfiguration.Add(processName, configuration);
    }

    public override void Register(string deviceName, string processName)
    {
        DeviceAudioConfiguration? deviceConfiguration;
        if (!_generalAudioConfiguration.TryGetValue(deviceName, out deviceConfiguration))
        {
            deviceConfiguration = new DeviceAudioConfiguration();
            _generalAudioConfiguration[deviceName] = deviceConfiguration;
        }

        if (deviceConfiguration.ContainsKey(processName))
        {
            return;
        }

        deviceConfiguration.Add(processName, new ProcessAudioConfiguration { VolumeLevel = _defaultVolumeLevel });
    }

    protected override async Task CreateFile() => await WriteToFile(_generalAudioConfiguration ?? EMPTY_GENERAL_AUDIO_CONFIGURATION);

    protected override async Task ReadFile()
    {
        await WaitForFileToBeAvailable();

        using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        GeneralAudioConfiguration? processConfigurations = null;
        try
        {
            processConfigurations = await JsonSerializer.DeserializeAsync(stream, GeneralAudioConfigurationSerializationContext.Default.GeneralAudioConfiguration);
        }
        catch (JsonException)
        {
        }

        _generalAudioConfiguration = processConfigurations ?? new GeneralAudioConfiguration();
    }

    protected override async Task WriteToFile(GeneralAudioConfiguration processConfiguration)
    {
        using var stream = File.OpenWrite(_filePath);
        using var streamWriter = new StreamWriter(stream, Encoding.UTF8);

        var data = JsonSerializer.Serialize(processConfiguration, GeneralAudioConfigurationSerializationContext.Default.GeneralAudioConfiguration);

        await _writeSemaphore.LockAsync(async () =>
        {
            await streamWriter.WriteAsync(data);
        });
    }

    public override async Task Prepare()
    {
        if (!File.Exists(_filePath))
        {
            await CreateFile();
        }

        await ReadFile();
    }

    public override async Task Reload()
    {
        await ReadFile();
    }

    public override async Task Save()
    {
        await WriteToFile(_generalAudioConfiguration);
    }
}
