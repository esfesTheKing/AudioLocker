using AudioLocker.Common.DataTypes;

namespace AudioLocker.Core.ConfigurationStorage.Abstract;

public interface IConfigurationStorage
{
    event Action OnConfigurationChanged;

    ProcessAudioConfiguration? Get(ReadOnlySpan<char> deviceName, ReadOnlySpan<char> processName);

    Task Prepare();
    Task Reload();
    Task Save();

    void Register(ReadOnlySpan<char> deviceName, string processName);
}
