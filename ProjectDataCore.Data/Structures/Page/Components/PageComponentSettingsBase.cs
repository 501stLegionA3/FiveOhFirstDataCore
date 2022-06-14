using ProjectDataCore.Data.Structures.Page.Components.Parameters;
using ProjectDataCore.Data.Structures.Page.Components.Scope;
using ProjectDataCore.Data.Structures.Policy;

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
    /// <summary>
    /// The qualified type name of the component to display.
    /// </summary>
    public string QualifiedTypeName { get; set; }

    /// <summary>
    /// The display name for this component.
    /// </summary>
    public string DisplayName { get; set; } = "Default Component";

    /// <summary>
    /// The parent settings object. May be null if the page reference is not null for a layout component.
    /// </summary>
    public Layout.LayoutNode? ParentNode { get; set; }
    /// <summary>
    /// The key for the <see cref="ParentNode"/>
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>
    /// The the containers for the <see cref="UserScope"/>s that this component gets data from (with ordering).
    /// </summary>
    public List<UserScopeListenerContainer> ScopeProviders { get; set; } = new();
    /// <summary>
    /// The containers for the <see cref="UserScope"/>s that this component sends data to (with ordering).
    /// </summary>
    public List<UserScopeProviderContainer> ScopeListeners { get; set; } = new();

    private List<UserScope>? orderedScopeProviders = null;
    public List<UserScope> GetOrderedScopeProviders()
	{
        bool recal = orderedScopeProviders is null;
        
        if (!recal)
		{
            recal = orderedScopeProviders!.Count != ScopeProviders.Count;
		}

        if (recal)
		{
            orderedScopeProviders = ScopeProviders
                .OrderBy(x => x.Order)
                .ToList(x => x.ProvidingScope);
        }

        // One final check.
        if (orderedScopeProviders is null)
            orderedScopeProviders = new();

        return orderedScopeProviders;
	}

    private List<UserScope>? orderedScopeListeners = null;
    public List<UserScope> GetOrderedScopeListeners()
    {
        bool recal = orderedScopeListeners is null;

        if (!recal)
        {
            recal = orderedScopeListeners!.Count != ScopeListeners.Count;
        }

        if (recal)
        {
            orderedScopeListeners = ScopeListeners
                .OrderBy(x => x.Order)
                .ToList(x => x.ListeningScope);
        }

        // One final check.
        if (orderedScopeListeners is null)
            orderedScopeListeners = new();

        return orderedScopeListeners;
    }
}
