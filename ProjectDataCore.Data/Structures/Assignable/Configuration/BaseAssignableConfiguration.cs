using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

public class BaseAssignableConfiguration : DataObject<Guid>
{
    public enum InputType
    {
        /// <summary>
        /// Only allow preset values to be selected.
        /// </summary>
        [Description("Static Only")]
        StaticOnly = -1,
        /// <summary>
        /// Allow both preset values and custom input.
        /// </summary>
        [Description("Both")]
        Both = 0,
        /// <summary>
        /// Only allow custom input.
        /// </summary>
        [Description("Freehand Only")]
        FreehandOnly = 1,
    }

    /// <summary>
    /// The name of the property to be selected. Must be unique.
    /// </summary>
    public string PropertyName { get; set; }
    /// <summary>
    /// A normalized version of the property name.
    /// </summary>
    public string NormalizedPropertyName { get; set; }
    /// <summary>
    /// The name of the type this configuration support. This value is assigned at creation
    /// and should not be changed.
    /// </summary>
    public string TypeName { get; set; }
    /// <summary>
    /// If this property allows for multiple of the values to be selected.
    /// </summary>
    public bool AllowMultiple { get; set; } = false;
    /// <summary>
    /// Determines the type of inputs allowed for this property.
    /// </summary>
    public InputType AllowedInput { get; set; } = InputType.StaticOnly;
    /// <summary>
    /// The list of values that use this configuration.
    /// </summary>
    public List<BaseAssignableValue> AssignableValues { get; set; } = new();
}
