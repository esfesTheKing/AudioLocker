using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

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
                audioProcs.ParentProcs[procName] = new Proc {
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
            string sessionName;

            try{
                sessionName = session.ToString();
            } catch (InvalidOperationException ex) {
                _logger.Debug($"Session exited during loop", ex);
                continue;
            }

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
            string sessionName;

            try {
                sessionName = session.ToString();
            } catch (InvalidOperationException ex) {
                _logger.Debug($"Session exited during loop", ex);
                continue;
            }

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

            try {
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

            int clevel;
            try {   
                clevel = (int) AudioUtilities.GetApplicationVolume(session.ProcessId);
            } catch (Exception ex) {
                _logger.Error($"Error with getting {sessionName}'s audio level.", ex);
                continue;
            }

            // Change audio only if current volume is different from the one set in the data file
            if (proc.vlevel == clevel) { continue; }

            try {
                AudioUtilities.SetApplicationVolume(session.ProcessId, proc.vlevel);
                _logger.Debug($"Successfuly set {sessionName}'s audio level.");
            } catch (Exception ex) { 
                _logger.Error($"Error with setting {sessionName}'s audio level.", ex);
            }
        }
    }
}