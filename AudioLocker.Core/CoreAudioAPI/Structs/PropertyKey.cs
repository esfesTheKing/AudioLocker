namespace AudioLocker.Core.CoreAudioAPI.Structs;

// https://learn.microsoft.com/en-us/windows/win32/api/wtypes/ns-wtypes-propertykey
// IDL Definition: "C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0\shared\WTypes.Idl"
public struct PropertyKey(Guid fmtid, int pid)
{
    public Guid fmtid = fmtid;
    public int pid = pid;
}
