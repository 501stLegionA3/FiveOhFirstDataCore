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
    // TODO: Tie in permissions system so each component
    // can limit the permission dynamically.

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

    /// <summary>
    /// A list of parameter components that are using this page component as a scope
    /// to retrive a <see cref="Account.DataCoreUser"/> from.
    /// </summary>
    public List<ParameterComponentSettingsBase> AttachedScopes { get; set; } = new();
}
