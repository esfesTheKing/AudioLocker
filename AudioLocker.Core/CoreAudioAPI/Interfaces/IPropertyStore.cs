using AudioLocker.Core.CoreAudioAPI.Structs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

[assembly: DisableRuntimeMarshalling]

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

// https://learn.microsoft.com/en-us/windows/win32/api/propsys/nn-propsys-ipropertystore
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\um\propsys.idl"
[GeneratedComInterface]
[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
public partial interface IPropertyStore
{
    int GetCount();

    PropertyKey GetAt(int iProp);

    PropVariant GetValue(ref PropertyKey key);

    void SaveValue(ref PropertyKey key, ref PropVariant propvar);

    void Commit();
}
