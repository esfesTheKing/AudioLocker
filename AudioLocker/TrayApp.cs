using AudioLocker.BL.Helpers;
using AudioLocker.Core.Loggers.Abstract;
using Microsoft.Win32;
using System.Diagnostics;

namespace AudioLocker;

public class AudioLockerTrayApp : ApplicationContext
{
    private readonly string REGISTRY_KEY_NAME = "AudioLocker";
    private readonly string EXECTUABLE_PATH = Application.ExecutablePath;

    private readonly ILogger _logger;
    private readonly string _settingsFile;

    private readonly NotifyIcon _trayIcon;
    private readonly UAC _uacHelper;

    public AudioLockerTrayApp(ILogger logger, string settingsFile)
    {
        _logger = logger;
        _settingsFile = settingsFile;

        _uacHelper = new UAC(logger);

        _trayIcon = new NotifyIcon()
        {
            Icon = Properties.Resources.ApplicationBorderIcon,
            ContextMenuStrip = new ContextMenuStrip()
            {
                Items = {
                    new ToolStripMenuItem("Startup On", null, (_, _) => SetRunOnStartup(true)),
                    new ToolStripMenuItem("Startup Off", null, (_, _) => SetRunOnStartup(false)),
                    new ToolStripMenuItem("Settings", null, OnOpenSettings),
                    new ToolStripMenuItem("Logs", null, OnOpenLogsFolder),
                    new ToolStripMenuItem("Exit", null, OnExit),
                }
            },
            Visible = true,
            Text = "AudioLocker",
        };
    }

    private void SetRegistryValue(bool addRegistryKey)
    {
        RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        if (registryKey is null)
        {
            _logger.Warning("Unable to get the run on startup registry key");
            return;
        }

        if (addRegistryKey)
        {
            registryKey.SetValue(REGISTRY_KEY_NAME, $"\"{EXECTUABLE_PATH}\" -s=\"{_settingsFile}\"");
            _logger.Info("AudioLocker is now running on startup");
            return;
        }

        registryKey.DeleteValue(REGISTRY_KEY_NAME, false);
        _logger.Info("AudioLocker is now not running on startup");
    }

    private bool SetRunOnStartup(bool runOnStartup)
    {
        var adminPrivilegesAvailable = _uacHelper.AreAdminPrivilegesAvailable();
        if (adminPrivilegesAvailable)
        {
            SetRegistryValue(runOnStartup);
            return false;
        }

        var successfullyStartedWithUAC = _uacHelper.StartProcessWithUACRights(EXECTUABLE_PATH, "-s", _settingsFile, "--startOnStartup", runOnStartup.ToString());
        if (successfullyStartedWithUAC)
        {
            Exit();
        }

        return successfullyStartedWithUAC;
    }

    public bool InitializeRunOnStartup(bool runOnStartup)
    {
        return !SetRunOnStartup(runOnStartup);
    }

    private void OnOpenSettings(object? sender, EventArgs e)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _settingsFile,
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    private void OnOpenLogsFolder(object? sender, EventArgs e)
    {
        var appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var logsDirectory = Path.Combine(appdataPath, "AudioLocker/logs");

        var startInfo = new ProcessStartInfo
        {
            FileName = logsDirectory,
            UseShellExecute = true
        };

        Process.Start(startInfo);

        _logger.Info($"Opening logs directory: {logsDirectory}");
    }

    private void OnExit(object? sender, EventArgs e)
    {
        Exit();
    }

    private void Exit()
    {
        // Hide tray icon, otherwise it will remain shown until user mouses over it
        _trayIcon.Visible = false;

        Application.Exit();
    }
}