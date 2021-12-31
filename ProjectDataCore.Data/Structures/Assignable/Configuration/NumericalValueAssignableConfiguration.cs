using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

[AssignableConfiguration("Integer")]
public class IntegerValueAssignableConfiguration : BaseAssignableConfiguration
{
    /// <summary>
    /// The avalible values to select from.
    /// </summary>
    public List<int> AvalibleValues { get; set; } = new();
}

[AssignableConfiguration("Decimal")]
public class DoubleValueAssignableConfiguration : BaseAssignableConfiguration
{
    /// <summary>
    /// The avalible values to select from.
    /// </summary>
    public List<double> AvalibleValues { get; set; } = new();
}
