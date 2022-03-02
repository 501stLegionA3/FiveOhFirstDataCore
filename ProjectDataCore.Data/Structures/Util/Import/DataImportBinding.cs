using System.Collections.Concurrent;

namespace ProjectDataCore.Data.Structures.Util.Import;

public class DataImportBinding
{
    public bool IsStatic { get; set; } = false;
    public string PropertyName { get; set; } = "";
    public ConcurrentDictionary<string, object> DataValues { get; set; } = new();
    public bool AutoConvert { get; set; } = false;

    public bool IsUsernameIdentifier { get; set; } = false;
    public bool IsUserIdIdentifier { get; set; } = false;

    public bool Password { get; set; } = false;
    public bool Email { get; set; } = false;
}
