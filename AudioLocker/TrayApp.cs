using AudioLocker.BL;
using AudioLocker.Core.Loggers.Abstract;
using AudioLocker.TrayAppTheme;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AudioLocker;

public class AudioLockerTrayApp : ApplicationContext
{
    private readonly ILogger _logger;
    private readonly string _settingsFile;

    private readonly Icon DarkThemeIcon = GetIcon("AudioLocker.Assets.Application Border.ico");
    private readonly Icon LightThemeIcon = GetIcon("AudioLocker.Assets.Application Border Dark.ico");

    private readonly NotifyIcon _trayIcon;

    public AudioLockerTrayApp(ILogger logger, string settingsFile)
    {
        _logger = logger;
        _settingsFile = settingsFile;

        _trayIcon = new NotifyIcon()
        {
            Icon = GetIconMatchingCurrentTheme(),
            ContextMenuStrip = new ContextMenuStrip()
            {
                Items = {
                    new ToolStripMenuItem("Enable On Boot", null, AddToRunOnStartup),
                    new ToolStripMenuItem("Disable On Boot", null, RemoveFromRunOnStartup),
                    new ToolStripMenuItem("Settings", null, OnOpenSettings),
                    new ToolStripMenuItem("Logs", null, OnOpenLogsFolder),
                    new ToolStripMenuItem("Exit", null, OnExit),
                },
                Renderer = GetRenderer()
            },
            Visible = true,
            Text = Constants.APP_NAME,
        };

        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    // NOTE:
    //  This callback will run on a thread that can not modify our application,
    //  thus we need to run it in a different thread using `Task.Run`.
    //
    //  https://learn.microsoft.com/en-us/dotnet/api/microsoft.win32.systemevents?view=windowsdesktop-9.0#remarks
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    private void OnUserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs @event)
    {
        Debouncer.Debounce(Constants.APP_NAME, () =>
        {
            if (_trayIcon.ContextMenuStrip is null)
            {
                return;
            }

            Application.SetColorMode(Application.SystemColorMode);
            _trayIcon.ContextMenuStrip.Renderer = GetRenderer();

            _trayIcon.Icon = GetIconMatchingCurrentTheme();
        });
    }

    private static ToolStripProfessionalRenderer GetRenderer()
    {
        if (Application.SystemColorMode == SystemColorMode.Dark)
        {
            return new ToolStripProfessionalRenderer(new TransparentSelectionDarkTheme());
        }

        return new ToolStripProfessionalRenderer(new TransparentSelectionClassicTheme());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Icon GetIconMatchingCurrentTheme()
    {
        return Application.SystemColorMode == SystemColorMode.Dark ? DarkThemeIcon : LightThemeIcon;
    }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    private static Icon GetIcon(string manifestResourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var stream = assembly.GetManifestResourceStream(manifestResourceName);
        return stream is null ? throw new Exception("No Icon was embedded in exe") : new Icon(stream);
    }

    private void AddToRunOnStartup(object? sender, EventArgs @event)
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);
        if (registryKey is null)
        {
            _logger.Warning("Unable to get the run on startup registry key");
            return;
        }

        registryKey.SetValue(Constants.APP_NAME, Application.ExecutablePath);
        _logger.Info("AudioLocker is now running on startup");
    }

    private void RemoveFromRunOnStartup(object? sender, EventArgs @event)
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);
        if (registryKey is null)
        {
            _logger.Warning("Unable to get the run on startup registry key");
            return;
        }

        registryKey.DeleteValue(Constants.APP_NAME, throwOnMissingValue: false);
        _logger.Info("AudioLocker is now not running on startup");
    }

    private void OnOpenSettings(object? sender, EventArgs @event)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _settingsFile,
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    private void OnOpenLogsFolder(object? sender, EventArgs @event)
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

    private void OnExit(object? sender, EventArgs @event)
    {
        Exit();
    }

    private void Exit()
    {
        // Hide tray icon, otherwise it will remain shown until user mouses over it
        _trayIcon.Visible = false;

        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;

        Application.Exit();
    }
}