using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page;

/// <summary>
/// Details the settings for a specific page based upon its
/// <see cref="Route"/>
/// </summary>
public class CustomPageSettings : DataObject<Guid>
{
    /// <summary>
    /// The Route for these settings.
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// The component to display on this page. It can be a layout
    /// component, meaning the page can have multiple different
    /// items displayed.
    /// </summary>
    public LayoutComponentSettings? Layout { get; set; }
    /// <summary>
    /// The ID for the layout in <see cref="Layout"/>
    /// </summary>
    public Guid? LayoutId { get; set; }
}
