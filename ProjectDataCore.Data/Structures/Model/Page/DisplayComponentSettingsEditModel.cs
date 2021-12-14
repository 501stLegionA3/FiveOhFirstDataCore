using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Page;

public class DisplayComponentSettingsEditModel : ParameterComponentSettingsEditModel
{
    /// <summary>
    /// Holds a change to <see cref="Structures.Page.Components.ParameterComponentSettingsBase.FormatString"/>
    /// </summary>
    public Optional<string?> FormatString { get; set; } = Optional.FromNoValue<string?>();
}
