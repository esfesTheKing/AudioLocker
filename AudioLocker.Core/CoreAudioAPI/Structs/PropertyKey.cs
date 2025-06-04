namespace AudioLocker.Core.CoreAudioAPI.Structs;

public struct PropertyKey(Guid fmtid, int pid)
{
    public Guid fmtid = fmtid;
    public int pid = pid;
}
