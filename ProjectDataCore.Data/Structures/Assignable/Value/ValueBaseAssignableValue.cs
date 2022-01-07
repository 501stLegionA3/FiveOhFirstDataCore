using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class ValueBaseAssignableValue<T> : BaseAssignableValue, IAssignableValue<T>
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public List<T> SetValue { get; set; } = new();

    public override object? GetValue()
    {
        return SetValue.FirstOrDefault();
    }

    public override object?[] GetValues()
    {
        return SetValue.ToArray(x => (object?)x);
    }
}
