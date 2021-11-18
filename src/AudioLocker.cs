using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.IO;
using System;

public static class AudioLocker
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public static void AudioLoop()
    {
        AudioHelper.Refresh();

        ProcContainer savedAudioProcs;

        //If someone is manually writing to the data file, we ignore that exception and just stop the current iteration of the funtion
        try {
            savedAudioProcs = FileHelper.ReadDataFile();
        } catch (IOException ex) { 
            _logger.Debug("An error accured with reading data file, skipping current iteration!", ex);
            return;
        }

        ProcContainer currentAudioProcs = AudioHelper.GetAudioProccess();

        if (AudioHelper.AreAudioProcsDifferent(ref savedAudioProcs, ref currentAudioProcs))
        {
            AudioHelper.AddNewAudioProcs(ref savedAudioProcs, ref currentAudioProcs);
            FileHelper.SaveAppsToDataFile(savedAudioProcs);
        }

        try {
            AudioHelper.SetAudioProcValues(savedAudioProcs);
        } catch (InvalidOperationException e) {
            _logger.Error("Caught error to not crash the bot", e);
        }
    }

    public static void StartAudioLocker()
    {
        if (!File.Exists(FileHelper.dataFilePath))
        {
            _logger.Debug("Data file was not found, creating a new one.");
            _logger.Debug($"Data file location: {FileHelper.dataFilePath}");
            FileHelper.SaveAppsToDataFile(new ProcContainer {
                ParentProcs = new Dictionary<string, Proc>(),
                ChildProcs = new Dictionary<string, Proc>()
            });
        }

        Thread myThread = new Thread(() => {
            Thread.CurrentThread.IsBackground = true;
            while(true)
            {
                Thread.Sleep(150);
                try {
                    AudioLoop();
                } catch (Exception ex) {
                    _logger.Error("An unknown error has occurred within the audio loop", ex);
                    throw;
                }
            }
        });

        _logger.Info("Starting audio loop.");
        myThread.Start();
    }

    public static void StartTrayIcon()
    {
        _logger.Info("Starting tray app.");
        Application.Run(new AudioLockerTrayApp());
    }

    public static void Start()
    {
        StartAudioLocker();
        StartTrayIcon();
    }
}