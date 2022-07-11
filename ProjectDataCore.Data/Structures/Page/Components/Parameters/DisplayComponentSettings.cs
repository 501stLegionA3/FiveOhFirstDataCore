
using ProjectDataCore.Data.Structures.Assignable.Render;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components.Parameters;

/// <summary>
/// Displays a data values.
/// </summary>
public class DisplayComponentSettings : ParameterComponentSettingsBase
{
    public string AuthorizedRaw { get; set; }
    public string UnAuthorizedRaw { get; set; }
}
