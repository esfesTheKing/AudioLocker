namespace AudioLocker.StartupArguments;

internal class StartupArguments
{
    public string SettingsFilePath { get; set; }

    public bool? StartOnStartup { get; set; }

    public int DefaultVolumeLevel { get; set; }
}
