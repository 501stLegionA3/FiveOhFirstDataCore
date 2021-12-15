using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;

public abstract class ParameterComponentSettingsBase : PageComponentSettingsBase
{
    // TODO switch over to using a DataCoreUserProperty insated.

    /// <summary>
    /// The name of the property to edit.
    /// </summary>
    public string PropertyToEdit { get; set; }

    /// <summary>
    /// True if the property name is a static property.
    /// </summary>
    /// <remarks>
    /// If this value is set to True, the <see cref="PropertyToEdit"/> field will look for properties
    /// that have been hard coded into the <see cref="Account.DataCoreUser"/> class.
    /// 
    /// If it is False, then it will search through assignable values.
    /// </remarks>
    public bool StaticProperty { get; set; }
    /// <summary>
    /// The label to display alongside this property.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// The component to pull a <see cref="Account.DataCoreUser"/> from. Leave as null to pull
    /// from the currently signed in user.
    /// </summary>
    public PageComponentSettingsBase? UserScope { get; set; }
    /// <summary>
    /// The ID of the <see cref="UserScope"/>
    /// </summary>
    public Guid? UserScopeId { get; set; }
}
