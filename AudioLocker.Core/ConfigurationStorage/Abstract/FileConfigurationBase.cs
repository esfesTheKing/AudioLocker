using AudioLocker.Common.DataTypes;
using AudioLocker.Core.Configuration.Abstract;

namespace AudioLocker.Core.ConfigurationStorage.Abstract;

public abstract class FileConfigurationBase : IConfigurationStorage
{
    private readonly int FILE_AVAILABLITY_SLEEP_IN_MILLISECONDS = 100;

    protected readonly string _filePath;

    private readonly FileSystemWatcher _watcher;

    public event Action? OnConfigurationChanged;

    public FileConfigurationBase(string filePath)
    {
        _filePath = filePath;

        var path = Path.GetDirectoryName(_filePath);
        if (string.IsNullOrEmpty(path))
        {
            path = ".";
        }

        _watcher = new FileSystemWatcher()
        {
            Path = path,
            Filter = Path.GetFileName(_filePath),
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        _watcher.Changed += OnChanged;
        _watcher.Renamed += OnFileNotFound;
        _watcher.Deleted += OnFileNotFound;
        _watcher.EnableRaisingEvents = true;
    }

    public abstract ProcessConfiguration? Get(string deviceName, string processName);
    public abstract void Register(string deviceName, string processName);
    public abstract Task Prepare();
    public abstract Task Reload();
    public abstract Task Save();
    protected abstract Task ReadFile();
    protected abstract Task WriteToFile<T>(T data);
    protected abstract Task CreateFile();

    private bool IsFileAvailable()
    {
        try
        {
            using var _ = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        catch (IOException)
        {
            return false;
        }

        return true;
    }

    protected async Task WaitForFileToBeAvailable()
    {
        while (!IsFileAvailable())
        {
            await Task.Delay(FILE_AVAILABLITY_SLEEP_IN_MILLISECONDS);
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        ReadFile();
        OnConfigurationChanged?.Invoke();
    }

    private void OnFileNotFound(object sender, FileSystemEventArgs e)
    {
        Prepare();
        ReadFile();
    }
}
