using System.Collections.Immutable;

namespace AudioLocker.Common.DataTypes;

public class ProcessConfigurationCollection : Dictionary<string, ProcessConfiguration>
{
    public static ImmutableDictionary<string, ProcessConfiguration> Empty => ImmutableDictionary<string, ProcessConfiguration>.Empty;
}
