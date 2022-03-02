using Microsoft.Extensions.Logging;

using ProjectDataCore.Data.Services.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Logging;
public class DataCoreLogScope : IDisposable, IDataCoreLogger
{
    private IDataCoreLogger Logger { get; set; }
    private DataCoreLog Scope { get; set; }

    internal DataCoreLogScope(IDataCoreLogger logger, DataCoreLog scope)
        => (Logger, Scope) = (logger, scope);

    public DataCoreLogScope CreateScope(DataCoreLog log, DataCoreLog? parentLog = null)
        => Logger.CreateScope(log, parentLog ?? Scope);

    public void Log(DataCoreLog log, DataCoreLog? parentLog = null)
        => Logger.Log(log, parentLog ?? Scope);

    public void Log(string message, LogLevel logLevel, Guid scope)
        => Log(new()
        {
            Message = message,
            LogLevel = logLevel,
            Scope = scope
        });

    public DataCoreLogScope CreateScope(string message, LogLevel logLevel, Guid scope)
        => CreateScope(new()
        {
            Message = message,
            LogLevel = logLevel,
            Scope = scope
        });

    public void Dispose()
    {
        // Do nothing (mmm using statements)
        Logger = null;
        Scope = null;
    }
}
