using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;
using System.Diagnostics;


Console.WriteLine("Hello, World!");
var enumerator = new MMDeviceEnumerator();

var collection = enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.Active);

foreach(var device in collection)
{
    Console.WriteLine(device);
    Console.WriteLine(device.Id);
    Console.WriteLine(device.State);
    Console.WriteLine(device.FriendlyName);
    var sessionEnumerator = device.AudioSessionManager.GetSessionEnumerator();

    var newCollection = new AudioSessionCollection(sessionEnumerator);

    foreach(var session in newCollection)
    {
        var newSession = (IAudioSessionControl2)session;
        Console.WriteLine(Process.GetProcessById((int)newSession.GetProcessId()).ProcessName);
        Console.WriteLine(newSession.GetProcessId());
    }
}
