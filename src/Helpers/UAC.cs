using System.Security.Principal;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System;

public static class UacHelper
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public static bool IsElevated = WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);

    public static void ExecutWithUAC(string args)
    {
        Process proc = new Process();
        proc.StartInfo.FileName = Application.ExecutablePath;
        proc.StartInfo.UseShellExecute = true;
        proc.StartInfo.Arguments = args;
        proc.StartInfo.Verb = "runas";

        _logger.Debug("Requesting UAC from user");
        proc.Start();
    }

    public static void SetRunAtStartup(bool val)
    {
        RegistryKey currentUser = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

        if (val)
        {
            currentUser.SetValue("AudioLocker", Application.ExecutablePath);
            _logger.Debug("Adding AudioLocker to startup.");
        }
        else
        {
            currentUser.DeleteValue("AudioLocker", false);
            _logger.Debug("Removing AudioLocker from startup.");
        }
    }

    public static bool GetAdminPrivileges(string args)
    {
        if (IsElevated)
            return true;

        try
        {
            ExecutWithUAC(args); // Start new process with UAC rights
        }
        catch
        {
            _logger.Debug("UAC request was cancelled by the user.");
            return false; // UAC cancelled by the user
        }
        Environment.Exit(0); // Kill current process
        return true; // Would never happen as this return will be blocked be the 'Environment.Exit'
    }
}