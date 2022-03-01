using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util.Import;

public class DataImportBinding
{
    public bool IsStatic { get; set; } = false;
    public string PropertyName { get; set; } = "";
    public string CSVValue { get; set; } = "";
    public object? DataValue { get; set; }
    public bool AutoConvert { get; set; } = false;

    public bool IsUsernameIdentifier { get; set; } = false;
    public bool IsUserIdIdentifier { get; set; } = false;
}
