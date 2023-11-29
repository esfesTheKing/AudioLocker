using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;

namespace AudioLocker.BL.Helpers;

[SupportedOSPlatform("windows")]
public static class ProcessHelper
{
    public static ManagementBaseObject? GetManagementBaseObject(ManagementObjectSearcher searcher)
    {
        foreach (var obj in searcher.Get())
        {
            return obj;
        }

        return null;
    }

    public static SelectQuery CreateQuery(uint processId)
    {
        var query = new SelectQuery("Win32_Process")
        {
            SelectedProperties = new StringCollection()
            {
                "Name",
                "ProcessId",
                "ParentProcessId"
            },
            Condition = $"ProcessId = {processId}"
        };

        return query;
    }

    public static uint GetPPID(uint processId)
    {
        var processQuery = CreateQuery(processId);
        var processSearcher = new ManagementObjectSearcher(processQuery);

        var process = GetManagementBaseObject(processSearcher);

        var parentQuery = CreateQuery((uint)process!["ParentProcessId"]);
        var parentSearcher = new ManagementObjectSearcher(parentQuery);

        var parent = GetManagementBaseObject(parentSearcher);
        if (parent == null)
        {
            return processId;
        }

        return (uint)parent["ProcessId"];
    }

    public static DateTime GetProcessStartTime(Process process)
    {
        try
        {
            return process.StartTime;
        }
        catch (Win32Exception)
        {
            return DateTime.MinValue;
        }
    }
}
