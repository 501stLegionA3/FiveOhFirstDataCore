using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

[AssignableConfiguration("Date & Time", typeof(DateTimeAssignableValue))]
public class DateTimeValueAssignableConfiguration : BaseAssignableConfiguration
{
    /// <summary>
    /// The avalible values to select from.
    /// </summary>
    public List<DateTime> AvalibleValues { get; set; } = new();
}

[AssignableConfiguration("Date Only", typeof(DateOnlyAssignableValue))]
public class DateOnlyValueAssignableConfiguration : BaseAssignableConfiguration
{
    /// <summary>
    /// The avalible values to select from.
    /// </summary>
    public List<DateOnly> AvalibleValues { get; set; } = new();
}

[AssignableConfiguration("Time Only", typeof(TimeOnlyAssignableValue))]
public class TimeOnlyValueAssignableConfiguration : BaseAssignableConfiguration
{
    /// <summary>
    /// The avalible values to select from.
    /// </summary>
    public List<TimeOnly> AvalibleValues { get; set; } = new();
}
