using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class StringAssignableValue : BaseAssignableValue
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public string SetValue { get; set; } = "";
}
