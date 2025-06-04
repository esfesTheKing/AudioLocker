using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

[GeneratedComInterface]
[Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8")]
public partial interface IAudioSessionEnumerator
{
    int GetCount();
    IAudioSessionControl GetSession(int SessionCount);
}
