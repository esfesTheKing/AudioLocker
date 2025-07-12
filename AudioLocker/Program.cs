namespace AudioLocker;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        var bootStrapper = new BootStrapper();
        bootStrapper.Run();
    }
}