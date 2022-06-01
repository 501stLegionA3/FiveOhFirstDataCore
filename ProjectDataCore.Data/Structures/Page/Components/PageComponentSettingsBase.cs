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
    /// If a user is required to be authroized to view this page. Setting this to <i><b>false</b></i> will
    /// make it a public page. Setting this to <i><b>true</b></i> without a policy assigned
    /// will make it a page that can be see by anyone who is logged in.
    /// </summary>
    public bool RequireAuth { get; set; } = true;

    /// <summary>
    /// The Authorization Policy for when <see cref="RequireAuth"/> is <i><b>true</b></i>.
    /// </summary>
    public DynamicAuthorizationPolicy? AuthorizationPolicy { get; set; }
    /// <summary>
    /// The key for the <see cref="AuthorizationPolicy"/>
    /// </summary>
    public Guid? AuthorizationPolicyKey { get; set; }

    /// <summary>
    /// The parent settings object. May be null if the page reference is not null for a layout component.
    /// </summary>
    public Layout.LayoutNode? ParentNode { get; set; }
    /// <summary>
    /// The key for the <see cref="ParentNode"/>
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>
    /// The <see cref="UserScope"/>s that this component gets data from.
    /// </summary>
    public List<UserScope> ScopeProviders { get; set; } = new();
    /// <summary>
    /// The <see cref="UserScope"/>s that this component sends data to.
    /// </summary>
    public List<UserScope> ScopeListeners { get; set; } = new();
}
