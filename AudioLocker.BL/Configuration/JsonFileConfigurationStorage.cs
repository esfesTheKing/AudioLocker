using AudioLocker.Common.DataTypes;
using AudioLocker.Core.ConfigurationStorage.Abstract;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace AudioLocker.BL.Configuration;

public class JsonFileConfigurationStorage : FileConfigurationBase
{
    private static readonly ImmutableDictionary<string, ProcessConfigurationCollection> EMPTY_PROCESS_CONFIGURATIONS = ImmutableDictionary<string, ProcessConfigurationCollection>.Empty;

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    private Dictionary<string, ProcessConfigurationCollection> _processConfigurations;
    private readonly int _defaultVolumeLevel;

    public JsonFileConfigurationStorage(string filePath, int defaultVolumeLevel)
        : base(filePath)
    {
        _defaultVolumeLevel = defaultVolumeLevel;
        _processConfigurations = new Dictionary<string, ProcessConfigurationCollection>();
    }

    private ProcessConfigurationCollection? GetConfiguration(string deviceName)
    {
        if (_processConfigurations.TryGetValue(deviceName, out var processConfiguration))
        {
            return processConfiguration;
        }

        return null;
    }

    public override ProcessConfiguration? Get(string deviceName, string processName)
    {
        var collection = GetConfiguration(deviceName);
        if (collection is null)
        {
            return null;
        }

        if (!collection.TryGetValue(processName, out var processConfiguration))
        {
            return null;
        }

        return processConfiguration;
    }

    public override void Set(string deviceName, string processName, ProcessConfiguration configuration)
    {
        var collection = GetConfiguration(deviceName);
        if (collection is null)
        {
            return;
        }

        collection.Add(processName, configuration);
    }

    public override void Register(string deviceName, string processName)
    {
        ProcessConfigurationCollection collection;
        if (!_processConfigurations.TryGetValue(deviceName, out collection!))
        {
            collection = new ProcessConfigurationCollection();
            _processConfigurations[deviceName] = collection;
        }

        if (collection.ContainsKey(processName))
        {
            return;
        }

        collection.Add(processName, new ProcessConfiguration { VolumeLevel = _defaultVolumeLevel });
    }

    protected override async Task CreateFile()
    {
        var castedProcessConfigurations = (IReadOnlyDictionary<string, ProcessConfigurationCollection>?)_processConfigurations;

        await WriteToFile(castedProcessConfigurations ?? EMPTY_PROCESS_CONFIGURATIONS);
    }

    private async Task<Dictionary<string, ProcessConfigurationCollection>> ReadFileHandleExceptions(Stream stream)
    {
        try
        {
            var processConfigurations = await JsonSerializer.DeserializeAsync<Dictionary<string, ProcessConfigurationCollection>>(stream);
            if (processConfigurations is not null)
            {
                return processConfigurations;
            }
        }
        catch (JsonException)
        {
        }

        return new Dictionary<string, ProcessConfigurationCollection>();
    }

    protected override async Task ReadFile()
    {
        await WaitForFileToBeAvailable();

        using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        _processConfigurations = await ReadFileHandleExceptions(stream);  
    }

    protected override async Task WriteToFile<T>(T processConfiguration)
    {
        var stream = File.OpenWrite(_filePath);
        using var streamWriter = new StreamWriter(stream, Encoding.UTF8);

        var data = JsonSerializer.Serialize(processConfiguration, _options);
        await streamWriter.WriteAsync(data);
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
