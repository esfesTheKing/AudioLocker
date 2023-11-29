﻿using AudioLocker.Common.DataTypes;

namespace AudioLocker.Core.Configuration.Abstract;

public interface IConfigurationStorage
{
    event Action OnConfigurationChanged;

    ProcessConfiguration? Get(string deviceName, string processName);
    void Set(string deviceName, string processName, ProcessConfiguration configuration);

    Task Prepare();
    Task Reload();
    Task Save();

    void Register(string deviceName, string processName);
}
