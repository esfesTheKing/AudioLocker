using AudioLocker.Core.CoreAudioAPI.Enums;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AudioLocker.Core.CoreAudioAPI.Interfaces;

[GeneratedComInterface]
[Guid("1BE09788-6894-4089-8586-9A2A6C265AC5")]
public partial interface IMMEndpoint
{
    EDataFlow GetDataFlow();
}
