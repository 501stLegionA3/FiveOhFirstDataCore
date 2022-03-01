using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util.Import;

public class DataImportConfiguration
{
    // Data Parse Configuration

    public bool CreateNewAccounts { get; set; } = true;
    public bool UpdateExistingAccounts { get; set; } = true;

    public ConcurrentDictionary<int, DataImportBinding> ValueBindings { get; init; } = new();

    // Imported Data Configuration

    public string? MultipleValueDelimiter { get; set; }
    public string StandardDelimiter { get; set; } = ",";

    // Imported Data

    public List<string[]> DataRows { get; set; } = new();
    public ConcurrentDictionary<int, HashSet<string>> UniqueValues { get; set; } = new();
}
