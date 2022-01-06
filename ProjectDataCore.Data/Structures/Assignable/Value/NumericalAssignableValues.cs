using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class IntegerAssignableValue : BaseAssignableValue, IAssignableValue<int>
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public List<int> SetValue { get; set; } = new();
}

public class DoubleAssignableValue : BaseAssignableValue, IAssignableValue<double>
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public List<double> SetValue { get; set; } = new();
}
