using ProjectDataCore.Data.Services.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Logging;
public class DataCoreLogScope : IDisposable, IDataCoreLogger
{
    private readonly IDataCoreLogger _logger;
    private readonly DataCoreLog _scope;

    internal DataCoreLogScope(IDataCoreLogger logger, DataCoreLog scope)
        => (_logger, _scope) = (logger, scope);

    public DataCoreLogScope CreateScope(DataCoreLog log, DataCoreLog? parentLog = null)
        => _logger.CreateScope(log, parentLog ?? _scope);

    public void Log(DataCoreLog log, DataCoreLog? parentLog = null)
        => _logger.Log(log, parentLog ?? _scope);

    public void Dispose()
    {
        // Do nothing (mmm using statements)
    }
}
