using ProjectDataCore.Data.Structures.Page.Components;
using ProjectDataCore.Data.Structures.Page.Components.Layout;
using ProjectDataCore.Data.Structures.Page.Components.Scope;

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
    /// The Display name for this page
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The component to display on this page. It can be a layout
    /// component, meaning the page can have multiple different
    /// items displayed.
    /// </summary>
    public LayoutNode? Layout { get; set; }
    /// <summary>
    /// The ID for the layout in <see cref="Layout"/>
    /// </summary>
    public Guid? LayoutId { get; set; }
    /// <summary>
    /// The user scopes for this page.
    /// </summary>
    public List<UserScope> UserScopes { get; set; } = new();

    /// <summary>
    /// Gets a list of component settings from all child layout nodes.
    /// </summary>
    /// <remarks>
    /// If this object has been pulled from the database without being
    /// proerply loaded, this method will not return a full list of
    /// components.
    /// </remarks>
    /// <returns>A <see cref="List{T}"/> of <see cref="PageComponentSettingsBase"/> for all child <see cref="LayoutNode"/>s
    /// that have a registered component.</returns>
    public List<PageComponentSettingsBase> GetChildComponents()
    {
        List<PageComponentSettingsBase> components = new();

        if (Layout is not null)
        {
            Stack<LayoutNode> nodeStack = new();
            nodeStack.Push(Layout);
            while (nodeStack.TryPop(out var node))
            {
                if (node.Component is not null)
                    components.Add(node.Component);

                foreach (var child in node.Nodes)
                    nodeStack.Push(child);
            }
        }

        return components;
	}
}
