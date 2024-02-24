using AudioLocker.Core.Loggers.Abstract;
using CommandLine;

namespace AudioLocker.StartupArguments;

internal class CommandLineStartupArguments
{
    [Option('s', "settingsFilePath", Required = false, Default = "settings.json")]
    public string SettingsFilePath { get; set; }

    [Option("startOnStartup", Required = false, Default = null)]
    public bool? StartOnStartup { get; set; }

    [Option("defaultVolumeLevel", Required = false, Default = 10)]
    public int DefaultVolumeLevel { get; set; }

    public static StartupArguments Parse(ILogger logger, string[] args)
    {
        var parserResult = Parser.Default.ParseArguments<CommandLineStartupArguments>(args);

        var parsingErrors = parserResult.Errors.ToList();
        if (parsingErrors.Count != 0)
        {
            var errors = string.Join('\n', parsingErrors.Select(error => error.Tag));
            logger.Warning($"Encountered errors while parsing command line arguments:\n {errors}");

            Application.Exit();
        }

        var commandLineStartupArgument = parserResult.Value;

        var startupArguments = new StartupArguments
        {
            SettingsFilePath = commandLineStartupArgument.SettingsFilePath,
            StartOnStartup = commandLineStartupArgument.StartOnStartup,
            DefaultVolumeLevel = commandLineStartupArgument.DefaultVolumeLevel,
        };

        return startupArguments;
    }
}
