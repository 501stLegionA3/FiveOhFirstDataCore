using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable;

public class DateTimeAssignable : DataObject<Guid>
{
    public DateTime DateTime { get; set; }
    public string Name { get; set; }
}
