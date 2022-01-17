using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Selector.User;

public class UserSelectComponentSettings : DataObject<Guid>
{
    /// <summary>
    /// A list of properties that the user select components should check.
    /// </summary>
    public List<string> Properties { get; set; } = new();
    /// <summary>
    /// Same length of the properties. Determines of the property values are static or not.
    /// </summary>
    public List<bool> IsStaticList { get; set; } = new();
    /// <summary>
    /// Same length of the properties. Provides the formats for the property values.
    /// </summary>
    public List<string> Formats { get; set; } = new();

    public LayoutComponentSettings LayoutComponent { get; set; }
    public Guid LayoutComponentId { get; set; }
}
