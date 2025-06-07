using AudioLocker.Core.Loggers.Abstract;
using Microsoft.Win32;
using System.Diagnostics;

namespace AudioLocker;

public class AudioLockerTrayApp : ApplicationContext
{
    private readonly string EXECTUABLE_PATH = Application.ExecutablePath;

    private readonly ILogger _logger;
    private readonly string _settingsFile;

    private readonly NotifyIcon _trayIcon;

    public AudioLockerTrayApp(ILogger logger, string settingsFile)
    {
        _logger = logger;
        _settingsFile = settingsFile;

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
            Text = Constants.APP_NAME,
        };
    }

    public void SetRunOnStartup(bool addRegistryKey)
    {
        RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);
        if (registryKey is null)
        {
            _logger.Warning("Unable to get the run on startup registry key");
            return;
        }

        if (addRegistryKey)
        {
            registryKey.SetValue(Constants.APP_NAME, string.Join(' ', new List<string> { EXECTUABLE_PATH, "-s", _settingsFile }));
            _logger.Info("AudioLocker is now running on startup");
            return;
        }

        registryKey.DeleteValue(Constants.APP_NAME, false);
        _logger.Info("AudioLocker is now not running on startup");
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
        var logsDirectory = Path.Combine(appdataPath, Constants.APP_NAME, "logs");

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