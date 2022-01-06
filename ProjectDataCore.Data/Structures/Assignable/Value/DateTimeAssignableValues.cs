using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class DateTimeAssignableValue : BaseAssignableValue, IAssignableValue<DateTime>
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public List<DateTime> SetValue { get; set; } = new();
}

public class DateOnlyAssignableValue : BaseAssignableValue, IAssignableValue<DateOnly>
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public List<DateOnly> SetValue { get; set; } = new();
}

public class TimeOnlyAssignableValue : BaseAssignableValue, IAssignableValue<TimeOnly>
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public List<TimeOnly> SetValue { get; set; } = new();
}
