namespace AudioLocker;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        ApplicationConfiguration.Initialize();

        var bootStrapper = new BootStrapper();
        bootStrapper.Run(args);
    }
}