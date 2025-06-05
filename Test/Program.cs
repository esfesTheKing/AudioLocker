using AudioLocker.Core.CoreAudioAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.Wrappers;
using System.Diagnostics;


Console.WriteLine("Hello, World!");
var enumerator = new MMDeviceEnumerator();

var collection = enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.DEVICE_STATE_ACTIVE);

foreach(var device in collection)
{
    Console.WriteLine(device);
    Console.WriteLine(device.Id);
    Console.WriteLine(device.State);
    Console.WriteLine(device.FriendlyName);
    Console.WriteLine(device.DataFlow);
    Console.WriteLine();
    var sessionCollection = device.AudioSessionManager.Sessions;

    foreach(var session in sessionCollection)
    {
        //var newSession = (IAudioSessionControl2)session;
        Console.WriteLine(Process.GetProcessById((int)session.ProcessId).ProcessName);
        Console.WriteLine(session.ProcessId);
        session.SimpleAudioVolume.SetVolume(10f / 100f);
        //Console.WriteLine(newSession.GetState());
    }
}
