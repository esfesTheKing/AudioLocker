using AudioLocker.Common.DataTypes;
using AudioLocker.Common.Extenstions;
using AudioLocker.Core.ConfigurationStorage.Abstract;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace AudioLocker.BL.Configuration;

public class JsonFileConfigurationStorage(string filePath, int defaultVolumeLevel) : FileConfigurationBase(filePath)
{
    private readonly int _defaultVolumeLevel = defaultVolumeLevel;

    private static readonly ImmutableDictionary<string, ProcessConfigurationCollection> EMPTY_PROCESS_CONFIGURATIONS = ImmutableDictionary<string, ProcessConfigurationCollection>.Empty;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    private Dictionary<string, ProcessConfigurationCollection> _processConfigurations = [];

    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

    private ProcessConfigurationCollection? GetConfiguration(string deviceName)
    {
        _processConfigurations.TryGetValue(deviceName, out var processConfiguration);

        return processConfiguration;
    }

    public override ProcessConfiguration? Get(string deviceName, string processName)
    {
        var collection = GetConfiguration(deviceName);

        ProcessConfiguration? processConfiguration = null;
        collection?.TryGetValue(processName, out processConfiguration);

        return processConfiguration;
    }

    public override void Set(string deviceName, string processName, ProcessConfiguration configuration)
    {
        var collection = GetConfiguration(deviceName);

        collection?.Add(processName, configuration);
    }

    public override void Register(string deviceName, string processName)
    {
        if (!_processConfigurations.TryGetValue(deviceName, out ProcessConfigurationCollection? collection))
        {
            collection = [];

            _processConfigurations[deviceName] = collection;
        }

        collection.TryAdd(processName, new ProcessConfiguration { VolumeLevel = _defaultVolumeLevel });
    }

    protected override async Task CreateFile()
    {
        var castedProcessConfigurations = (IReadOnlyDictionary<string, ProcessConfigurationCollection>?)_processConfigurations;

        await WriteToFile(castedProcessConfigurations ?? EMPTY_PROCESS_CONFIGURATIONS);
    }

    private async Task<Dictionary<string, ProcessConfigurationCollection>> ReadFileHandleExceptions(Stream stream)
    {
        Dictionary<string, ProcessConfigurationCollection>? processConfigurations = null;
        try
        {
            processConfigurations = await JsonSerializer.DeserializeAsync<Dictionary<string, ProcessConfigurationCollection>>(stream);
        }
        catch (JsonException) { }

        return processConfigurations ?? [];
    }

    protected override async Task ReadFile()
    {
        await WaitForFileToBeAvailable();

        using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        _processConfigurations = await ReadFileHandleExceptions(stream);
    }

    protected override async Task WriteToFile<T>(T processConfiguration)
    {
        await _writeSemaphore.LockAsync(async () => {
            using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Write, FileShare.Read);
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8);

            var data = JsonSerializer.Serialize(processConfiguration, _options);

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
        await WriteToFile(_processConfigurations);
    }
}
