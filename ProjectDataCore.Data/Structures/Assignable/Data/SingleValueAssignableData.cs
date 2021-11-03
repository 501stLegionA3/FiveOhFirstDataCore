using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable;

public class SingleValueAssignableData : DataObject<Guid>
{
    public string Value { get; set; }
}
