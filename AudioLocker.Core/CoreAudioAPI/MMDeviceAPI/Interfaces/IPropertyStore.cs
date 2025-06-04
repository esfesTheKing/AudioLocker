using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Structs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

[assembly: DisableRuntimeMarshalling]

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;

[GeneratedComInterface]
[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
public partial interface IPropertyStore
{
    void GetCount(out int count);
    PropertyKey GetAt(int index);
    PropVariant GetValue(ref PropertyKey key);
    void SaveValue(ref PropertyKey key, ref PropVariant variant);
    void Commit();
}
