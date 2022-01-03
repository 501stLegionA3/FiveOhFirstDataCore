using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ProjectDataCore.Data.Structures.Assignable.Configuration.BaseAssignableConfiguration;

namespace ProjectDataCore.Data.Structures.Model.Assignable;

public class AssignableConfigurationEditModel<T>
{
    public string? PropertyName { get; set; } = null;
    public bool? AllowMultiple { get; set; } = null;
    public InputType? AllowedInput { get; set; } = null;
    public List<T>? AllowedValues { get; set; } = null;
}
