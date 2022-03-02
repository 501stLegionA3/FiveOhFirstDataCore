using Microsoft.Extensions.Logging;

using ProjectDataCore.Data.Structures.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Logging;
public class InstanceLogger : IInstanceLogger
{
    public ConcurrentDictionary<Action<DataCoreLog>, (Guid, LogLevel)> Loggers { get; init; } = new();
    public HashSet<Guid> Keys { get; init; } = new();

    public DataCoreLogScope CreateScope(DataCoreLog log, DataCoreLog? parentLog = null)
    {
        Log(log, parentLog);

        return new(this, log);
    }

    public DataCoreLogScope CreateScope(string message, LogLevel logLevel, Guid scope)
        => CreateScope(new()
        {
            Message = message,
            LogLevel = logLevel,
            Scope = scope
        });

    public void Log(DataCoreLog log, DataCoreLog? parentLog = null)
    {
        if (parentLog is not null)
            parentLog.ChildLogs.Add(log);

        if(log.Scope != default)
            PunchToListeners(log);
    }

    public void Log(string message, LogLevel logLevel, Guid scope)
        => Log(new()
        {
            Message = message,
            LogLevel = logLevel,
            Scope = scope
        });

    public Guid Register(Action<DataCoreLog> receiver, LogLevel minLogLevel = LogLevel.Information)
    {
        var key = Guid.NewGuid();
        while (!Keys.Contains(key))
            key = Guid.NewGuid();

        Keys.Add(key);
        Loggers[receiver] = (key, minLogLevel);

        return key;
    }

    public bool Register(Action<DataCoreLog> receiver, LogLevel minLogLevel, Guid scope)
    {
        if (!Keys.Contains(scope))
            return false;

        Keys.Add(scope);
        Loggers[receiver] = (scope, minLogLevel);

        return true;
    }

    public void Unregister(Action<DataCoreLog> reciver)
    {
        if (Loggers.TryRemove(reciver, out var data))
            Keys.Remove(data.Item1);
    }

    private void PunchToListeners(DataCoreLog log)
    {
        _ = Task.Run(() =>
        {
            foreach (var pair in Loggers)
            {
                if (pair.Value.Item1 == log.Scope
                    && pair.Value.Item2 >= log.LogLevel)
                    pair.Key.Invoke(log);
            }
        });
    }
}
