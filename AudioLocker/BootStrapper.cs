using AudioLocker.BL.Audio;
using AudioLocker.BL.ConfigurationStorage;
using AudioLocker.BL.Loggers;
using AudioLocker.Core.ConfigurationStorage.Abstract;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using Serilog;

using AudioLockerILogger = AudioLocker.Core.Loggers.Abstract.ILogger;

namespace AudioLocker;

internal class BootStrapper
{
    private readonly AudioLockerILogger _logger;
    private DeviceConfigurator? _deviceConfigurator;

    public BootStrapper()
    {
        _logger = GetLogger();
    }

    public void Run()
    {
        using var mutext = new Mutex(true, Constants.APP_NAME, out bool createdNew);
        if (!createdNew)
        {
            _logger.Error("Another instance of this app is already running.");
            return;
        }

        InitializeLoggingOfUnhandledExcpetions();
        InitializeAudioSetup();
        InitializeTrayApp();
    }

    private void InitializeAudioSetup()
    {
        var storage = GetStorage();

        storage.Prepare().Wait();

        var enumerator = new MMDeviceEnumerator();
        _deviceConfigurator = new DeviceConfigurator(_logger, storage, enumerator);

        _deviceConfigurator.Initialize();
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

    private void InitializeTrayApp()
    {
        ApplicationConfiguration.Initialize();
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        Application.SetColorMode(SystemColorMode.System);
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        var trayApp = new AudioLockerTrayApp(_logger, ResolveSettingsFilePath());
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
                        $"{appdataRoaming}\\{Constants.APP_NAME}\\logs\\{date}.log",
                        retainedFileCountLimit: null,
                        outputTemplate: outputTemplate,
                        fileSizeLimitBytes: 5L * 1024 * 1024, // 5MB
                        shared: true
                    );
                },
                sinkMapCountLimit: 1 // Keep only the sink of the current day
            )
            .CreateLogger();

        return new SerilogLogger(log);
    }

    private IConfigurationStorage GetStorage()
    {
        // TODO: Move defualt volume level to settings file
        var storage = new JsonFileConfigurationStorage(ResolveSettingsFilePath(), Constants.DEFAULT_AUDIO_SESSION_VOLUME_LEVEL);

        return storage;
    }

    private string ResolveSettingsFilePath()
    {
        var localPath = Path.Combine(Application.StartupPath, "settings.json");
        if (Path.Exists(localPath))
        {
            return localPath;
        }

        var homePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".{Constants.APP_NAME}", "settings.json");
        if (Path.Exists(homePath))
        {
            return homePath;
        }

        return localPath;
    }
}
