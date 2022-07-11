
using ProjectDataCore.Data.Structures.Assignable.Render;
using ProjectDataCore.Data.Structures.Page.Components.Scope;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page;

public abstract class ParameterComponentSettingsBase : PageComponentSettingsBase
{
    /// <summary>
    /// The <see cref="AssignableValueRenderer"/> objects this parameter component has configured.
    /// </summary>
    public List<AssignableValueRenderer> AssignableValueRenderers { get; set; } = new();
}
