using AudioLocker.BL.Audio;
using AudioLocker.BL.Configuration;
using AudioLocker.BL.Loggers;
using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.Loggers.Abstract;
using AudioLocker.StartupArguments;
using log4net;
using log4net.Config;

using t_StartupArguments = AudioLocker.BL.StartupArguments;

[assembly: XmlConfigurator(Watch = true, ConfigFile = "./App.config")]

namespace AudioLocker;

internal class BootStrapper
{
    private readonly string LOGGER_NAME = "Logger";
    private DeviceConfigurationHandler? _deviceConfigurationHandler;

    public void Run(string[] args)
    {
        var logger = GetLogger();
        var arguments = ParseArguments(logger, args);

        using var mutext = new Mutex(true, Constants.APP_NAME, out bool createdNew);
        if (!createdNew)
        {
            logger.Error("Another instance of this app is already running.");
            return;
        }

        InitializeLoggingOfUnhandledExcpetions(logger);
        InitializeAudioSetup(logger, arguments);
        InitializeTrayApp(logger, arguments);
    }

    private void InitializeAudioSetup(ILogger logger, t_StartupArguments arguments)
    {
        var storage = GetStorage(arguments);

        storage.Prepare().Wait();

        var enumerator = new MMDeviceEnumerator();
        _deviceConfigurationHandler = new DeviceConfigurationHandler(logger, storage, enumerator);

        _deviceConfigurationHandler.Initialize();
    }

    private t_StartupArguments ParseArguments(ILogger logger, string[] args)
    {
        return CommandLineStartupArguments.Parse(logger, args);
    }

    private void InitializeLoggingOfUnhandledExcpetions(ILogger logger)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, @event) =>
        {
            var exception = (Exception)@event.ExceptionObject;
            logger.Fatal("Unknown exception has caused the app to crash!", exception);
        };

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, @event) =>
        {
            logger.Fatal("Unknown exception has caused the app to crash!", @event.Exception);
        };
    }

    private void InitializeTrayApp(ILogger logger, t_StartupArguments arguments)
    {
        ApplicationConfiguration.Initialize();

        var trayApp = new AudioLockerTrayApp(logger, arguments.SettingsFilePath);

        if (arguments.StartOnStartup is not null)
        {
            trayApp.SetRunOnStartup((bool)arguments.StartOnStartup);
        }

        Application.Run(trayApp);
    }

    private ILogger GetLogger()
    {
        var log = LogManager.GetLogger(LOGGER_NAME);

        return new Log4NetLogger(log);
    }

    private IConfigurationStorage GetStorage(t_StartupArguments arguments)
    {
        var storage = new JsonFileConfigurationStorage(arguments.SettingsFilePath, arguments.DefaultVolumeLevel);

        return storage;
    }
}
