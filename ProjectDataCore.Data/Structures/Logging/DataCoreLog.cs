using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Logging;
public class DataCoreLog : DataObject<Guid>
{
    public string Message { get; set; }
    public LogLevel LogLevel { get; set; }
    public Guid? Scope { get; set; }
    public List<DataCoreLog> ChildLogs { get; set; } = new();
}
