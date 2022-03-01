using Microsoft.Extensions.Logging;

using ProjectDataCore.Data.Structures.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Logging;
public interface IDataCoreLogger
{
    public void Log(DataCoreLog log, DataCoreLog? parentLog = null);
    public DataCoreLogScope CreateScope(DataCoreLog log, DataCoreLog? parentLog = null);
}
