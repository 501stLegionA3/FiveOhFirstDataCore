using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Page;

public class ParameterComponentSettingsEditModel
{
    /// <summary>
    /// Holds a change to <see cref="Structures.Page.Components.ParameterComponentSettingsBase.PropertyToEdit"/>
    /// </summary>
    public string? PropertyToEdit { get; set; }
    /// <summary>
    /// Holds a change to <see cref="Structures.Page.Components.ParameterComponentSettingsBase.StaticProperty"/>
    /// </summary>
    public bool? StaticProperty { get; set; }
    /// <summary>
    /// Holds a change to <see cref="Structures.Page.Components.ParameterComponentSettingsBase.Label"/>
    /// </summary>
    public Optional<string?> Label { get; set; } = Optional.FromNoValue<string?>();
}
