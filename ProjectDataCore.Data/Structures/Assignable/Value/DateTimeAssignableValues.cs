using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class DateTimeAssignableValue : BaseAssignableValue
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public DateTime SetValue { get; set; } = DateTime.UtcNow;
}

public class DateOnlyAssignableValue : BaseAssignableValue
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public DateOnly SetValue { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

public class TimeOnlyAssignableValue : BaseAssignableValue
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public TimeOnly SetValue { get; set; } = TimeOnly.FromDateTime(DateTime.UtcNow);
}
