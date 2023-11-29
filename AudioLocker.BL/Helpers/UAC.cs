using AudioLocker.Core.Loggers.Abstract;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Principal;

namespace AudioLocker.BL.Helpers;

[SupportedOSPlatform("windows")]
public class UAC
{
    private readonly ILogger _logger;
    private bool? _isElevated;

    public UAC(ILogger logger)
    {
        _logger = logger;
    }

    public bool AreAdminPrivilegesAvailable()
    {
        _isElevated ??= CheckIfAdminPrivilegesAreAvailable();

        return (bool)_isElevated;
    }

    public bool StartProcessWithUACRights(string executablePath, params string[] args)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = true,
            Arguments = string.Join(' ', args),
            Verb = "runas"
        };

        try
        {
            Process.Start(startInfo);
        }
        catch
        {
            return false;
        }

        return true;
    }

    private static bool CheckIfAdminPrivilegesAreAvailable()
    {
        WindowsIdentity identity;
        try
        {
            identity = WindowsIdentity.GetCurrent();
        }
        catch (SecurityException)
        {
            return false;
        }

        var principal = new WindowsPrincipal(identity);

        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
