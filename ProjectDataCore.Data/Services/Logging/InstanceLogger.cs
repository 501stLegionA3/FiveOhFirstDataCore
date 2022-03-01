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

    public DataCoreLogScope CreateScope(DataCoreLog log, DataCoreLog? parentLog = null)
    {
        Log(log, parentLog);

        return new(this, log);
    }

    public void Log(DataCoreLog log, DataCoreLog? parentLog = null)
    {
        if (parentLog is not null)
            parentLog.ChildLogs.Add(log);


    }

    public Guid Register(Action<DataCoreLog> receiver, LogLevel minLogLevel)
    {
        throw new NotImplementedException();
    }

    public void Register(Action<DataCoreLog> receiver, LogLevel minLogLevel, Guid scope)
    {
        throw new NotImplementedException();
    }

    public void Unregister(Action<DataCoreLog> reciver)
    {
        throw new NotImplementedException();
    }

    private void PunchToListeners(DataCoreLog log)
    {

    }
}
