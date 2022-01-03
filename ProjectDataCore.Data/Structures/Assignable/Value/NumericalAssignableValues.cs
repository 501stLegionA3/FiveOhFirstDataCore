using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class IntegerAssignableValue : BaseAssignableValue
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public int SetValue { get; set; } = 0;
}

public class DoubleAssignableValue : BaseAssignableValue
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public double SetValue { get; set; } = 0.0;
}
