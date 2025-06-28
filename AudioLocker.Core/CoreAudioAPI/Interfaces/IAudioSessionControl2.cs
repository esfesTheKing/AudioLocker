using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessioncontrol2
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\audiopolicy.idl"
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
