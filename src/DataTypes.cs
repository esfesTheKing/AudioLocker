using System.Collections.Generic;

public class Proc
{
    public int vlevel { get; set; }
    public bool isPersistent { get; set; }
}

public class ProcContainer
{
    public IDictionary<string, Proc> ParentProcs { get; set; }
    public IDictionary<string, Proc> ChildProcs { get; set; }
}