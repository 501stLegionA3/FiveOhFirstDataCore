using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

[AssignableConfiguration("Date & Time", typeof(DateTimeAssignableValue))]
public class DateTimeValueAssignableConfiguration : ValueBaseAssignableConfiguration<DateTime>
{

}

[AssignableConfiguration("Date Only", typeof(DateOnlyAssignableValue))]
public class DateOnlyValueAssignableConfiguration : ValueBaseAssignableConfiguration<DateOnly>
{

}

[AssignableConfiguration("Time Only", typeof(TimeOnlyAssignableValue))]
public class TimeOnlyValueAssignableConfiguration : ValueBaseAssignableConfiguration<TimeOnly>
{

}
