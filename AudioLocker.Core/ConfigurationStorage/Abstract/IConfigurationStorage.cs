using AudioLocker.Common.DataTypes;

namespace AudioLocker.Core.ConfigurationStorage.Abstract;

public interface IConfigurationStorage
{
    event Action OnConfigurationChanged;

    ProcessAudioConfiguration? Get(string deviceName, string processName);

    Task Prepare();
    Task Reload();
    Task Save();

    void Register(string deviceName, string processName);
}
