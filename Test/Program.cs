using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Enums;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;



Console.WriteLine("Hello, World!");
var enumerator = new MMDeviceEnumerator();

var collection = enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.Active);

foreach(var device in collection)
{
    Console.WriteLine(device);
    Console.WriteLine(device.Id);
    Console.WriteLine(device.State);
    Console.WriteLine(device.FriendlyName);

}
