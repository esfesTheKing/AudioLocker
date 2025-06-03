// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;



Console.WriteLine("Hello, World!");
var enumerator = new MMDeviceEnumerator();

var collection = enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.Active);

foreach(var device in collection)
{
    Console.WriteLine(device);
    Console.WriteLine(device.Id);
}
