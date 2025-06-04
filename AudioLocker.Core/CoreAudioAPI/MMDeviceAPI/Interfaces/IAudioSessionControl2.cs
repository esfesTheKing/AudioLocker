using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D")]
public partial interface IAudioSessionControl2 : IAudioSessionControl
{
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetSessionIdentifier();
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetSessionInstanceIdentifier();
    uint GetProcessId();
    [return: MarshalAs(UnmanagedType.Bool)]
    bool IsSystemSoundsSession();
    void SetDuckingPreference([MarshalAs(UnmanagedType.Bool)] bool optOut);
}
