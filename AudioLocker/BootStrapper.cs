using AudioLocker.BL.Audio;
using AudioLocker.BL.Configuration;
using AudioLocker.BL.Loggers;
using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.Core.Loggers.Abstract;
using AudioLocker.StartupArguments;
using log4net;
using log4net.Config;
//using NAudio.CoreAudioApi;
using t_StartupArguments = AudioLocker.BL.StartupArguments;

[assembly: XmlConfigurator(Watch = true, ConfigFile = "./App.config")]

namespace AudioLocker;

internal class BootStrapper
{
    private readonly string LOGGER_NAME = "Logger";

    public void Run(string[] args)
    {
        var logger = GetLogger();

        using var mutext = new Mutex(true, Constants.APP_NAME, out bool createdNew);
        if (!createdNew)
        {
            logger.Error("Another instance of this app is already running.");
            return;
        }

        var arguments = ParseArguments(logger, args);

        var thread = new Thread(async () => await InitializeAudioSetup(logger, arguments));
        thread.IsBackground = true;
        thread.SetApartmentState(ApartmentState.MTA);

        thread.Start();

        InitializeTrayApp(logger, arguments);
    }

    private async Task InitializeAudioSetup(ILogger logger, t_StartupArguments arguments)
    {
        var storage = GetStorage(arguments);

        await storage.Prepare();

        var enumerator = new MMDeviceEnumerator();
        var audioManager = new AudioManager(logger, storage, enumerator);

        var notificationClient = new MMNotificationClient(logger, enumerator, audioManager);

        enumerator.RegisterEndpointNotificationCallback(notificationClient);

        await audioManager.Initialize();
    }

    private t_StartupArguments ParseArguments(ILogger logger, string[] args)
    {
        return CommandLineStartupArguments.Parse(logger, args);
    }

    private void InitializeLoggingOfUnhandledExcpetions(ILogger logger)
    {
        AppDomain.CurrentDomain.UnhandledException += (object _, UnhandledExceptionEventArgs @event) =>
        {
            var exception = (Exception)@event.ExceptionObject;
            logger.Fatal("Unknown exception has caused the app to crash!", exception);
        };

        Application.ThreadException += (object _, ThreadExceptionEventArgs @event) =>
        {
            logger.Fatal("Unknown exception has caused the app to crash!", @event.Exception);
        };

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    }

    private void InitializeTrayApp(ILogger logger, t_StartupArguments arguments)
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        ApplicationConfiguration.Initialize();

        InitializeLoggingOfUnhandledExcpetions(logger);

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
