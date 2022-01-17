using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Page;

public class ParameterComponentSettingsEditModel
{
    /// <summary>
    /// Holds a change to <see cref="ParameterComponentSettingsBase.PropertyToEdit"/>
    /// </summary>
    public string? PropertyToEdit { get; set; }
    /// <summary>
    /// Holds a change to <see cref="ParameterComponentSettingsBase.StaticProperty"/>
    /// </summary>
    public bool? StaticProperty { get; set; }
    /// <summary>
    /// Holds a change to <see cref="ParameterComponentSettingsBase.Label"/>
    /// </summary>
    public Optional<string?> Label { get; set; } = Optional.FromNoValue<string?>();
    /// <summary>
    /// Holds a change to <see cref="Structures.Page.Components.ParameterComponentSettingsBase.FormatString"/>
    /// </summary>
    public Optional<string?> FormatString { get; set; } = Optional.FromNoValue<string?>();
    /// <summary>
    /// Holds a change to <see cref="ParameterComponentSettingsBase.UserScopeId"/>
    /// </summary>
    public Optional<Guid?> UserScope { get; set; } = Optional.FromNoValue<Guid?>();
}
