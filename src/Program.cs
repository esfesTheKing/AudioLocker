using System.Reflection;
using System.Linq;
using System;

[assembly: log4net.Config.XmlConfigurator(Watch=true, ConfigFile="App.config")]

// ! Compile command -> dotnet publish -r win-x64 -c Release --self-contained true
/*
    TODO: Package the program with an installer, so it will copy it self to disk C
*/

class Program
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public static void ProcessArgs(string[] args)
    {
        _logger.Debug("Processing args");

        /*
            --SON -> start up on
            --SOF -> start up off
        */
        if (args.Contains("--SON"))
            UacHelper.SetRunAtStartup(true);
        if (args.Contains("--SOF"))
            UacHelper.SetRunAtStartup(false);
    }

    public static void Main(string[] args)
    {
        _logger.Info("Starting AudioLocker.");
        ProcessArgs(args);
        AudioLocker.Start();
    }
}