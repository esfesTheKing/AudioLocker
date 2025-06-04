namespace AudioLocker.Core.CoreAudioAPI.MMDeviceAPI.Structs;

public struct PropertyKey(Guid fmtid, int pid)
{
    public Guid fmtid = fmtid;
    public int pid = pid;
}
