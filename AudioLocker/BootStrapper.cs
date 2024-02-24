using AudioLocker.BL;
using AudioLocker.BL.Configuration;
using AudioLocker.BL.Loggers;
using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.Loggers.Abstract;
using AudioLocker.StartupArguments;
using log4net;
using log4net.Config;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using t_StartupArguments = AudioLocker.StartupArguments.StartupArguments;

[assembly: XmlConfigurator(Watch = true, ConfigFile = "./App.config")]

namespace AudioLocker;

internal class BootStrapper
{
    private readonly string LOGGER_NAME = "Logger";
    private const uint DEVICE_INVALIDATED_ERORR = 0x88890004;

    public void Run(string[] args)
    {
        var logger = GetLogger();
        var arguments = ParseArguments(logger, args);

        var thread = new Thread(async () => await InitializeAudioLoop(logger, arguments));
        thread.IsBackground = true;
        thread.SetApartmentState(ApartmentState.MTA);

        thread.Start();

        InitializeTrayApp(logger, arguments);
    }

    private t_StartupArguments ParseArguments(ILogger logger, string[] args)
    {
        return CommandLineStartupArguments.Parse(logger, args);
    }

    private async Task InitializeAudioLoop(ILogger logger, t_StartupArguments arguments)
    {
        var storage = new JsonFileConfigurationStorage(arguments.SettingsFilePath, arguments.DefaultVolumeLevel);

        await storage.Prepare();

        var enumerator = new MMDeviceEnumerator();
        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            SetupMMDevice(logger, storage, device);

            device.AudioSessionManager.OnSessionCreated += (object _, IAudioSessionControl iSession) =>
            {
                var session = new AudioSessionControl(iSession);

                OnSessionCreated(logger, storage, device, session);
            };

            await storage.Save();
        }

        TaskScheduler.UnobservedTaskException += (object? _, UnobservedTaskExceptionEventArgs e) =>
        {
            var exception = e.Exception as Exception;
            logger.Fatal("Unknown exception has caused the app to crash!", exception);
            e.SetObserved();
        };
    }

    private void InitializeTrayApp(ILogger logger, t_StartupArguments arguments)
    {
        var trayApp = new AudioLockerTrayApp(logger, arguments.SettingsFilePath);

        if (arguments.StartOnStartup is not null && !trayApp.InitializeRunOnStartup((bool)arguments.StartOnStartup))
        {
            return;
        }

        AppDomain.CurrentDomain.UnhandledException += (object _, UnhandledExceptionEventArgs e) =>
        {
            var exception = e.ExceptionObject as Exception;
            logger.Fatal("Unknown exception has caused the app to crash!", exception);
        };

        TaskScheduler.UnobservedTaskException += (object? _, UnobservedTaskExceptionEventArgs e) =>
        {
            var exception = e.Exception as Exception;
            logger.Fatal("Unknown exception has caused the app to crash!", exception);
            e.SetObserved();
        };

        Application.Run(trayApp);
    }

    private ILogger GetLogger()
    {
        var log = LogManager.GetLogger(LOGGER_NAME);
        return new Log4NetLogger(log);
    }

    private void OnSessionCreated(ILogger logger, IConfigurationStorage storage, MMDevice device, AudioSessionControl session)
    {
        uint pid;
        try
        {
            pid = session.GetProcessID;
        }
        catch (COMException e)
        {
            var statusCode = unchecked((uint)e.ErrorCode);
            if (statusCode != DEVICE_INVALIDATED_ERORR)
            {
                logger.Warning("Unknown error has accord while trying to configure new session: ", e);
            }

            return;
        }

        var deviceName = device.FriendlyName;
        ConfigureSession(logger, storage, session, deviceName);

        storage.Save();
    }

    private void SetupMMDevice(ILogger logger, IConfigurationStorage storage, MMDevice device)
    {
        var deviceName = device.FriendlyName;

        var sessions = device.AudioSessionManager.Sessions;

        for (int i = 0; i < sessions.Count; ++i)
        {
            var session = sessions[i];
            if (session == null)
            {
                continue;
            }

            ConfigureSession(logger, storage, session, deviceName);
        }
    }

    private static void ConfigureSession(ILogger logger, IConfigurationStorage storage, AudioSessionControl session, string deviceName)
    {
        var process = Process.GetProcessById((int)session.GetProcessID);
        var processName = process.ProcessName;

        storage.Register(deviceName, processName);

        var audioSessionEventHandler = new AudioSessionEventHandler(logger, storage, session, deviceName, processName);

        var simpleAudioVolumeInterface = session.SimpleAudioVolume;
        audioSessionEventHandler.OnVolumeChanged(simpleAudioVolumeInterface.Volume, simpleAudioVolumeInterface.Mute);

        session.RegisterEventClient(audioSessionEventHandler);
        logger.Info($"New session was configured: {processName}");
    }
}
