using System.Collections.Generic;
using System.Security.Principal;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.Win32;
using System.Linq;
using System.IO;
using System;

public static class UacHelper
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public static bool IsElevated = WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);

    public static void ExecutWithUAC(string args) {
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
        RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        if (val) {
            rk.SetValue("AudioLocker", Application.ExecutablePath);
            _logger.Debug("Adding AudioLocker to startup.");
        }
        else {
            rk.DeleteValue("AudioLocker", false);
            _logger.Debug("Removing AudioLocker from startup.");
        }
    }

    public static bool GetAdminPrivileges(string args) {
        if (IsElevated)
            return true;

        try {
            ExecutWithUAC(args); // Start new process with UAC rights
        } catch {
            _logger.Debug("UAC request was cancelled by the user.");
            return false; // UAC cancelled by the user
        }
        Environment.Exit(0); // Kill current process
        return true; // Would never happen as this return will be blocked be the 'Environment.Exit'
    }
}

public static class FileHelper
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public const string dataFile = "data.json";

    public static ProcContainer ReadDataFile()
    {
        string jsonData = File.ReadAllText(dataFile);
        return JsonSerializer.Deserialize<ProcContainer>(jsonData);
    }

    public static void SaveAppsToDataFile(ProcContainer audioProcs)
    {
        JsonSerializerOptions options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
        };

        string jsonString = JsonSerializer.Serialize(audioProcs, options);
        _logger.Info("Saving audio processes to data file.");
        File.WriteAllText(dataFile, jsonString);
    }
}

public static class AudioHelper
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public static IList<AudioSession> currentAudioSessions;
    public static Dictionary<string, int> parentProcs;

    public static void Refresh()
    {
        currentAudioSessions = AudioUtilities.GetAllSessions();
        parentProcs = GetAllParentProcs();
    }

    public static bool AreAudioProcsDifferent(ref ProcContainer oldAudioProcs, ref ProcContainer newAudioProcs)
    {
        bool parentChanges = newAudioProcs.ParentProcs.Keys.Except(oldAudioProcs.ParentProcs.Keys).Any();
        bool childChanges = newAudioProcs.ChildProcs.Keys.Except(oldAudioProcs.ChildProcs.Keys).Any();

        return parentChanges || childChanges;
    }

    public static void AddNewAudioProcs(ref ProcContainer audioProcs, ref ProcContainer newAudioProcs)
    {
        foreach((string procName, Proc proc) in newAudioProcs.ParentProcs)
        {
            if(!audioProcs.ParentProcs.ContainsKey(procName)) {
                _logger.Info($"Added {procName} to AudioProcs.");
                audioProcs.ParentProcs[procName] = new Proc{
                    vlevel = 10,
                    isPersistent = true
                };
            }
        }

        foreach((string procName, Proc proc) in newAudioProcs.ChildProcs)
        {
            if(!audioProcs.ChildProcs.ContainsKey(procName)) {
                _logger.Info($"Added {procName} to AudioProcs.");
                audioProcs.ChildProcs[procName] = new Proc {
                    vlevel = 10,
                    isPersistent = true
                };
            }
        }
    }

    public static Dictionary<string, int> GetAllParentProcs()
    {
        Dictionary<string, int> parentProcs = new Dictionary<string, int>();
        foreach (AudioSession session in currentAudioSessions)
        {
            string sessionName = session.ToString();
            if (!parentProcs.ContainsKey(sessionName))
                parentProcs[sessionName] = session.ProcessId;

            // Parent process should have the lower process id from all the process with the same name
            if (session.ProcessId < parentProcs[sessionName])
                parentProcs[sessionName] = session.ProcessId;
        }
        return parentProcs;
    }

    public static ProcContainer GetAudioProccess()
    {
        ProcContainer audioProcs = new ProcContainer
        {
            ParentProcs = new Dictionary<string, Proc>(),
            ChildProcs = new Dictionary<string, Proc>()
        };

        foreach (AudioSession session in currentAudioSessions)
        {
            string sessionName = session.ToString();

            if (session.ProcessId == parentProcs[sessionName])
                audioProcs.ParentProcs[sessionName] = null;
            else
                audioProcs.ChildProcs[sessionName] = null;
        }

        return audioProcs;
    }

    public static void SetAudioProcValues(ProcContainer audioProcs)
    {
        foreach (AudioSession session in currentAudioSessions)
        {
            string sessionName;

            try{
                sessionName = session.ToString();
            } catch (InvalidOperationException ex) {
                _logger.Debug($"Session exited during loop", ex);
                continue;
            }

            Proc proc;

            if (session.ProcessId == parentProcs[sessionName])
                proc = audioProcs.ParentProcs[sessionName];
            else
                proc = audioProcs.ChildProcs[sessionName];

            if (!proc.isPersistent)
                continue;

            // Change audio only if current volume is different from the one set in the data file
            if (proc.vlevel != (int) AudioUtilities.GetApplicationVolume(session.ProcessId)) 
            {
                try {
                    AudioUtilities.SetApplicationVolume(session.ProcessId, proc.vlevel);
                    _logger.Debug($"Successfuly set {sessionName}'s audio level.");
                } catch (Exception ex) { 
                    _logger.Error($"Error with setting {sessionName}'s audio level.", ex);
                }
            }
        }
    }
}