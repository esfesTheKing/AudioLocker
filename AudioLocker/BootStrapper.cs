using AudioLocker.BL;
using AudioLocker.BL.Configuration;
using AudioLocker.BL.Loggers;
using AudioLocker.Core.Configuration.Abstract;
using AudioLocker.Core.Loggers.Abstract;
using log4net;
using log4net.Config;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;

[assembly: XmlConfigurator(Watch = true, ConfigFile = "./App.config")]

namespace AudioLocker;

internal class BootStrapper
{
    private readonly string CONFIGURATION_STORAGE_FILE_PATH = "settings.json";
    private readonly string LOGGER_NAME = "Logger";
    private readonly int DEFAULT_VOLUME_LEVEL = 10;

    public void Run(string[] args)
    {
        var logger = GetLogger();

        var thread = new Thread(async () => await InitializeAudioLoop(logger))
        {
            IsBackground = true
        };

        thread.SetApartmentState(ApartmentState.MTA);
        thread.Start();

        InitializeTrayApp(logger, args);
    }

    private async Task InitializeAudioLoop(ILogger logger)
    {
        var storage = new JsonFileConfigurationStorage(CONFIGURATION_STORAGE_FILE_PATH, DEFAULT_VOLUME_LEVEL);

        await storage.Prepare();

        var enumerator = new MMDeviceEnumerator();
        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            SetupMMDevice(device, logger, storage);

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

    private void InitializeTrayApp(ILogger logger, string[] args)
    {
        var trayApp = new AudioLockerTrayApp(logger, CONFIGURATION_STORAGE_FILE_PATH);

        if (args.Length > 0 && bool.TryParse(args[0], out bool runOnStartup))
        {
            var shouldContinueOwnInitialization = trayApp.InitializeRunOnStartup(runOnStartup);
            if (!shouldContinueOwnInitialization)
            {
                return;
            }
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
            if (statusCode == 0x88890004)
            {
                return;
            }

            logger.Warning("Unknown error has accord while trying to configure new session: ", e);
            return;
        }

        var deviceName = device.FriendlyName;
        ConfigureSession(logger, storage, session, deviceName);

        // Note: This is a potential race condition, needs further testing
        storage.Save();
    }

    private void SetupMMDevice(MMDevice device, ILogger logger, IConfigurationStorage storage)
    {
        var deviceName = device.FriendlyName;

        var sessions = device.AudioSessionManager.Sessions;
        var registeredSessions = new List<uint>();

        for (int i = 0; i < sessions.Count; ++i)
        {
            var session = sessions[i];
            if (session == null)
            {
                continue;
            }

            if (registeredSessions.Contains(session.GetProcessID))
            {
                continue;
            }

            ConfigureSession(logger, storage, session, deviceName);
            registeredSessions.Add(session.GetProcessID);
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
