namespace AudioLocker.BL;

public class StartupArguments
{
    public string SettingsFilePath { get; set; }

    public bool? StartOnStartup { get; set; }

    public int DefaultVolumeLevel { get; set; }
}