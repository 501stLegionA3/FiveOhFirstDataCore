
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page;

public abstract class ParameterComponentSettingsBase : PageComponentSettingsBase
{
    // TODO Get rid of all of this.

    /// <summary>
    /// The name of the property to edit.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// True if the property name is a static property.
    /// </summary>
    /// <remarks>
    /// If this value is set to True, the <see cref="PropertyName"/> field will look for properties
    /// that have been hard coded into the <see cref="Account.DataCoreUser"/> class.
    /// 
    /// If it is False, then it will search through assignable values.
    /// </remarks>
    public bool StaticProperty { get; set; }
}
