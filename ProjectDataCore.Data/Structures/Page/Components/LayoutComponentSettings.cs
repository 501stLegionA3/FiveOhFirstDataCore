using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;

/// <summary>
/// A layout component that contains more component settings.
/// </summary>
public class LayoutComponentSettings : PageComponentSettingsBase
{
    /// <summary>
    /// The max ammount of child components this layout component can have.
    /// </summary>
    /// <remarks>
    /// This count indicates the size of a layout grid. The 2x2 would have a max size of 4,
    /// while a 3x3 would have a max size of 9.
    /// </remarks>
    public int MaxChildComponents { get; set; }
    /// <summary>
    /// The child components for this layout component. Each child has an order property
    /// to organize values properly.
    /// </summary>
    public List<PageComponentSettingsBase> ChildComponents { get; set; } = new();

    /// <summary>
    /// The page this layout belongs to. Will be null unless
    /// this layout is the top level layout of the page.
    /// </summary>
    public CustomPageSettings? ParentPage { get; set; }
    /// <summary>
    /// The ID of the page in <see cref="ParentPage"/>
    /// </summary>
    public Guid? ParentPageId { get; set; }
}
