using Microsoft.Extensions.Logging;

using ProjectDataCore.Data.Structures.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Logging;
public interface IInstanceLogger : IDataCoreLogger
{
    public Guid Register(Action<DataCoreLog> receiver, LogLevel minLogLevel);
    public bool Register(Action<DataCoreLog> receiver, LogLevel minLogLevel, Guid scope);
    public void Unregister(Action<DataCoreLog> reciver);
}
