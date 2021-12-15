using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util;

public class DataCoreUserProperty : DataObject<Guid>
{
    /// <summary>
    /// Name of the property to get.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// If the property is a static property or not.
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// The order to display this property in.
    /// </summary>
    /// <remarks>
    /// The lower this value, the further to the left or top the value will be
    /// displayed. Equal order values will be sorted alphabetically by the
    /// property name.
    /// </remarks>
    public int Order { get; set; }

    /// <summary>
    /// The format string for a property.
    /// </summary>
    /// <remarks>
    /// If left null, the default display is {0} - or a simple
    /// conversion to a string.
    /// </remarks>
    public string FormatString { get; set; } = "{0}";

    /// <summary>
    /// The alias for a property value to use. 0 is the
    /// primary (non-alias) value.
    /// </summary>
    public int Alias { get; set; }


    #region FK
    public Guid? RosterComponentUserListingDisplayId { get; set; }
    public Guid? RosterComponentDefaultDisplayId { get; set; }
    #endregion
}
