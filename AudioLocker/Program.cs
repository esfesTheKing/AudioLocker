namespace AudioLocker;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var bootStrapper = new BootStrapper();
        bootStrapper.Run(args);
    }
}