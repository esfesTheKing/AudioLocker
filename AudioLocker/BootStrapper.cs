using AudioLocker.BL.Audio;
using AudioLocker.BL.ConfigurationStorage;
using AudioLocker.BL.Loggers;
using AudioLocker.Core.ConfigurationStorage.Abstract;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using AudioLocker.StartupArguments;
using Serilog;

using AudioLockerILogger = AudioLocker.Core.Loggers.Abstract.ILogger;
using t_StartupArguments = AudioLocker.BL.StartupArguments;

namespace AudioLocker;

internal class BootStrapper
{
    private readonly AudioLockerILogger _logger;
    private DeviceConfigurator? _deviceConfigurator;

    public BootStrapper()
    {
        _logger = GetLogger();
    }

    public void Run(string[] args)
    {
        var arguments = ParseArguments(args);

        using var mutext = new Mutex(true, Constants.APP_NAME, out bool createdNew);
        if (!createdNew)
        {
            _logger.Error("Another instance of this app is already running.");
            return;
        }

        InitializeLoggingOfUnhandledExcpetions();
        InitializeAudioSetup(arguments);
        InitializeTrayApp(arguments);
    }

    private void InitializeAudioSetup(t_StartupArguments arguments)
    {
        var storage = GetStorage(arguments);

        storage.Prepare().Wait();

        var enumerator = new MMDeviceEnumerator();
        _deviceConfigurator = new DeviceConfigurator(_logger, storage, enumerator);

        _deviceConfigurator.Initialize();
    }

    private t_StartupArguments ParseArguments(string[] args)
    {
        return CommandLineStartupArguments.Parse(_logger, args);
    }

    private void InitializeLoggingOfUnhandledExcpetions()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, @event) =>
        {
            var exception = (Exception)@event.ExceptionObject;
            _logger.Fatal("Unknown exception has caused the app to crash!", exception);
        };

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, @event) =>
        {
            _logger.Fatal("Unknown exception has caused the app to crash!", @event.Exception);
        };
    }

    private void InitializeTrayApp(t_StartupArguments arguments)
    {
        ApplicationConfiguration.Initialize();

        var trayApp = new AudioLockerTrayApp(_logger, arguments.SettingsFilePath);

        if (arguments.StartOnStartup is not null)
        {
            trayApp.SetRunOnStartup((bool)arguments.StartOnStartup);
        }

        Application.ApplicationExit += CleanupOnApplicationExit;

        Application.Run(trayApp);
    }

    private void CleanupOnApplicationExit(object? sender, EventArgs @event)
    {
        _logger.Info("Running cleanup before exiting...");
        _deviceConfigurator?.Dispose();

        Application.ApplicationExit -= CleanupOnApplicationExit;
        _logger.Info("Finished running cleanup");
    }

    private AudioLockerILogger GetLogger()
    {
        var appdataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}] [{Class}.{Method}] {Message}{NewLine}{Exception}";

        var log = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Map(
                _ => DateTime.Now.ToString("yyyy.MM.dd"),
                (date, wt) =>
                {
                    wt.File(
                        $"{appdataRoaming}\\{Constants.APP_NAME}\\logs\\{date}.txt",
                        retainedFileCountLimit: null,
                        outputTemplate: outputTemplate,
                        fileSizeLimitBytes: 5L * 1024 * 1024 // 5MB
                    );
                }
            )
            .CreateLogger();

        return new SerilogLogger(log);
    }

    private IConfigurationStorage GetStorage(t_StartupArguments arguments)
    {
        var storage = new JsonFileConfigurationStorage(arguments.SettingsFilePath, arguments.DefaultVolumeLevel);

        return storage;
    }
}
