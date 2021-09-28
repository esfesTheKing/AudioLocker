using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.IO;
using System;

public class AudioLockerTrayApp : ApplicationContext
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private NotifyIcon trayIcon;

    public AudioLockerTrayApp()
    {
        ContextMenuStrip MyContextMenue = new ContextMenuStrip();

        MyContextMenue.Items.AddRange(
                    new ToolStripMenuItem[] {
                        new ToolStripMenuItem("Startup On", null, SetStartupTrue),
                        new ToolStripMenuItem("Startup Off", null, SetStartupFalse),
                        new ToolStripMenuItem("Settings", null, OpenSettings),
                        new ToolStripMenuItem("Logs", null, OpenLogs),
                        new ToolStripMenuItem("Exit", null, Exit),
                    });

        trayIcon = new NotifyIcon()
        {
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
            ContextMenuStrip = MyContextMenue,
            Visible = true,
            Text = "AudioLocker",
        };
    }

    private void SetStartup(string arg, bool val)
    {
        bool hasAdmin = UacHelper.GetAdminPrivileges(arg);
        if (hasAdmin)
            UacHelper.SetRunAtStartup(val);
    }

    private void SetStartupTrue(object sender, EventArgs e)
    {
        SetStartup("--SON", true);
    }

    private void SetStartupFalse(object sender, EventArgs e)
    {
        SetStartup("--SOF", false);
    }

    private void OpenSettings(object sender, EventArgs e)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = $"{Directory.GetCurrentDirectory()}\\{FileHelper.dataFile}";
        startInfo.UseShellExecute = true;

        Process.Start(startInfo);
    }

    private void OpenLogs(object sender, EventArgs e)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C start %APPDATA%/AudioLocker/logs";
        startInfo.UseShellExecute = true;

        Process.Start(startInfo);
    }

    private void Exit(object sender, EventArgs e)
    {
        // Hide tray icon, otherwise it will remain shown until user mouses over it
        trayIcon.Visible = false;

        Environment.Exit(0);
    }
}