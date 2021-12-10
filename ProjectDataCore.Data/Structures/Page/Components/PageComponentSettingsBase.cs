using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;

/// <summary>
/// Base settings for all component setting objects.
/// </summary>
public class PageComponentSettingsBase : DataObject<Guid>
{
    // TODO: Tie in permissions system so each component
    // can limit the permission dynamically.

    /// <summary>
    /// The qualified type name of the component to display.
    /// </summary>
    public string QualifiedTypeName { get; set; }

    /// <summary>
    /// Used to determine a components position in a layout element.
    /// </summary>
    /// <remarks>
    /// The order integer moves left to right, top to bottom as the elemnts are accessed.
    /// 
    /// For a 2x2 grid square, the item at index 1 would be in the top right corner, while
    /// the item at index 2 would be in the bottom left.
    /// </remarks>
    public int Order { get; set; }

    /// <summary>
    /// The parent settings object. May be null if the page reference is not null for a layout component.
    /// </summary>
    public LayoutComponentSettings? ParentLayout { get; set; }
    /// <summary>
    /// The key for the <see cref="ParentLayout"/>
    /// </summary>
    public Guid? ParentLayoutId { get; set; }
}
