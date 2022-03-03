using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System.Collections.Concurrent;

namespace ProjectDataCore.Data.Structures.Util.Import;

public class DataImportBinding
{
    public bool IsStatic { get; set; } = false;
    public string PropertyName { get; set; } = "";
    public ConcurrentDictionary<string, object> DataValues { get; set; } = new();
    public ConcurrentDictionary<string, (AssignableConfigurationValueEditModel, BaseAssignableConfiguration)> DataValueModels { get; set; } = new();
    public bool AutoConvert { get; set; } = false;

    public bool IsUsernameIdentifier { get; set; } = false;
    public bool IsUserIdIdentifier { get; set; } = false;

    public bool PasswordIdentifier { get; set; } = false;
    public bool EmailIdentifier { get; set; } = false;
}
