using AudioLocker.Common.DataTypes;
using AudioLocker.Common.Extenstions;
using AudioLocker.Core.ConfigurationStorage.Abstract;
using System.Text;
using System.Text.Json;

namespace AudioLocker.BL.ConfigurationStorage;

public class JsonFileConfigurationStorage(string filePath, int defaultVolumeLevel) : FileConfigurationStorageBase<GeneralAudioConfiguration>(filePath)
{
    private GeneralAudioConfiguration _generalAudioConfiguration = [];
    private readonly int _defaultVolumeLevel = defaultVolumeLevel;
    private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

    private DeviceAudioConfiguration? GetConfiguration(ReadOnlySpan<char> deviceName)
    {
        _generalAudioConfiguration.TryGetValue(deviceName, out var processConfiguration);

        return processConfiguration;
    }

    public override ProcessAudioConfiguration? Get(ReadOnlySpan<char> deviceName, ReadOnlySpan<char> processName)
    {
        DeviceAudioConfiguration? collection = GetConfiguration(deviceName);

        ProcessAudioConfiguration? processConfiguration = null;
        collection?.TryGetValue(processName, out processConfiguration);

        return processConfiguration;
    }

    public override void Register(ReadOnlySpan<char> deviceName, string processName)
    {
        if (!_generalAudioConfiguration.TryGetValue(deviceName, out DeviceAudioConfiguration? collection))
        {
            collection = [];

            _generalAudioConfiguration.Add(deviceName.ToString(), collection);
        }

        collection!.TryAdd(processName, new ProcessAudioConfiguration { VolumeLevel = _defaultVolumeLevel });
    }

    protected override async Task CreateFile()
    {
        await WriteToFile(_generalAudioConfiguration ?? []);
    }

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

        _generalAudioConfiguration = processConfigurations ?? [];
    }

    protected override async Task WriteToFile(GeneralAudioConfiguration processConfiguration)
    {
        await _writeSemaphore.LockAsync(async () =>
        {
            using var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8);

            var data = JsonSerializer.Serialize(processConfiguration, GeneralAudioConfigurationSerializationContext.Default.GeneralAudioConfiguration);

            await streamWriter.WriteAsync(data);
            await streamWriter.FlushAsync();
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
