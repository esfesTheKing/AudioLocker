using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Implementations;
using AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Structs;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

[assembly: DisableRuntimeMarshalling]

namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Interfaces;
[GeneratedComInterface]
[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
public partial interface IPropertyStore
{
    void Commit();
    void GetAt(int index, out PropertyKey key);
    void GetCount(out int count);
    void GetValue(ref PropertyKey key, out PROPVARIANT variant);
    void SaveValue(ref PropertyKey key, ref PROPVARIANT variant);
}
