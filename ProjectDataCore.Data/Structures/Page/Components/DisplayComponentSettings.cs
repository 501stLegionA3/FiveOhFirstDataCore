using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;

/// <summary>
/// Displays a single data value.
/// </summary>
public class DisplayComponentSettings : ParameterComponentSettingsBase
{
    /// <summary>
    /// The format string for a static property.
    /// </summary>
    public string? FormatString { get; set; }
}
