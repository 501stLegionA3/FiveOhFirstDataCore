using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

[AssignableConfiguration("Integer", typeof(IntegerAssignableValue))]
public class IntegerValueAssignableConfiguration : BaseAssignableConfiguration
{
    /// <summary>
    /// The avalible values to select from.
    /// </summary>
    public List<int> AvalibleValues { get; set; } = new();
}

[AssignableConfiguration("Decimal", typeof(DoubleAssignableValue))]
public class DoubleValueAssignableConfiguration : BaseAssignableConfiguration
{
    /// <summary>
    /// The avalible values to select from.
    /// </summary>
    public List<double> AvalibleValues { get; set; } = new();
}
